using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace fidelizPlus_back.Payment
{
    using AppDomain;
    using Services;

    public class PaymentHandler
    {
        private RequestDelegate Next { get; }
        private PaymentMonitor Monitor { get; }

        public PaymentHandler(
            RequestDelegate next,
            PaymentMonitor monitor
        )
        {
            Next = next;
            Monitor = monitor;
        }

        private bool WaitPayment(int purchaseId)
        {
            bool done = false;
            try
            {
                Thread.Sleep(20000);
            }
            catch (ThreadInterruptedException)
            {
                done = !Monitor.IsMonitored(purchaseId);
            }
            return done;
        }

        private void ThreadVsTask(Thread thread, Func<Task> asyncFunc)
        /*
        Run the two parameters simultaneously.
        If the thread finish first, the asynfunc run is interrupted.
        Else the thread is interrupted.
        */
        {
            Thread threadWrapper = new Thread(toKill =>
            {
                try
                {
                    thread.Start();
                    thread.Join();
                    ((Thread)toKill).Interrupt();
                }
                catch (ThreadInterruptedException)
                { }
            });
            Func<Task> taskWrapper = async () =>
            {
                threadWrapper.Start(Thread.CurrentThread);
                await asyncFunc();
                ((Thread)threadWrapper).Interrupt();
                thread.Interrupt();
            };
            try
            {
                taskWrapper().Wait();
            }
            catch (ThreadInterruptedException)
            { }
        }

        private async Task<int> ReadPurchaseId(NiceWebSocket webSocket, RelatedToBothService<Purchase, PurchaseDTO> purchaseService)
        {
            string purchaseIdStr = await webSocket.Read();
            int purchaseId;
            try
            {
                purchaseId = Int32.Parse(purchaseIdStr);
                Purchase purchase = purchaseService.FindEntity(purchaseId);
                if (purchase.PayingTime != null)
                {
                    throw new AppException("Already payed");
                }
            }
            catch (Exception e)
            {
                object errorObject = e is AppException ? ((AppException)e).Content : "Not a number";
                await webSocket.Error(JsonSerializer.Serialize(errorObject));
                throw new AppException(errorObject);
            }
            return purchaseId;
        }

        private async Task HandleWebSocketRequest(HttpContext context, RelatedToBothService<Purchase, PurchaseDTO> purchaseService)
        {
            NiceWebSocket webSocket = new NiceWebSocket(context);
            int purchaseId = await ReadPurchaseId(webSocket, purchaseService);
            bool payed = false;
            bool timeout = false;
            Thread waitPayment = new Thread(() =>
            {
                payed = WaitPayment(purchaseId);
                timeout = true;
            });
            Monitor.Add(purchaseId, waitPayment);
            ThreadVsTask(waitPayment, () => webSocket.Read());
            await webSocket.Close(
                payed ? "Payed" :
                timeout ? "Timeout" :
                "Interrupted"
            );
        }

        public async Task Invoke(HttpContext context, RelatedToBothService<Purchase, PurchaseDTO> purchaseService)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    await HandleWebSocketRequest(context, purchaseService);
                }
                else
                {
                    throw new AppException("Only for ws protocol", 400);
                }
            }
            else
            {
                await Next(context);
            }
        }
    }
}
