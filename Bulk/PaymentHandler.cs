using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace fidelizPlus_back
{
    using AppDomain;
    using DTO;
    using Services;

    public class PaymentHandler
    {
        private RequestDelegate Next { get; }
        private PaymentMonitor Monitor { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }

        public PaymentHandler(
            RequestDelegate next,
            PaymentMonitor monitor,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService
        )
        {
            Next = next;
            Monitor = monitor;
            PurchaseService = purchaseService;
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

        private async Task<int> ReadPurchaseId(NiceWebSocket webSocket)
        {
            string purchaseIdStr = await webSocket.Read();
            int purchaseId;
            try
            {
                purchaseId = Int32.Parse(purchaseIdStr);
                Purchase purchase = PurchaseService.FindEntity(purchaseId);
                if (purchase.PayingTime != null)
                {
                    throw new AppException("Already payed");
                }
            }
            catch (Exception e)
            {
                object errorObject = e is AppException ? ((AppException)e).Content : "Not a number";
                await webSocket.Close(JsonSerializer.Serialize(errorObject));
                throw new AppException(errorObject);
            }
            return purchaseId;
        }

        private async Task HandleWebSocketRequest(HttpContext context)
        {
            NiceWebSocket webSocket = new NiceWebSocket(context);
            int purchaseId = await ReadPurchaseId(webSocket);
            bool payed = false;
            bool timeout = false;
            Thread waitPayment = new Thread(() =>
            {
                payed = WaitPayment(purchaseId);
                timeout = true;
            });
            Monitor.Add(purchaseId, waitPayment);
            ThreadVsTask(waitPayment, () => webSocket.Read());
            await webSocket.Send(
                payed ? "Payed" :
                timeout ? "Timeout" :
                "Interrupted"
            );
            await webSocket.Close();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    await HandleWebSocketRequest(context);
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

    public static class PaymentHandlerExtensions
    {
        public static IApplicationBuilder UsePaymentHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PaymentHandler>();
        }
    }
}
