using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace fidelizPlus_back.Payment
{
	using AppDomain;
	using Services;

	public class PaymentHandler
	{
		private RequestDelegate Next { get; }
		private Monitor Monitor { get; }

		public PaymentHandler(
			RequestDelegate next,
			Monitor monitor
		)
		{
			Next = next;
			Monitor = monitor;
		}

		private async Task<int> ReadPurchaseId(NiceWebSocket webSocket, PurchaseService purchaseService)
		{
			string purchaseIdStr = await webSocket.Read();
			int purchaseId;
			try
			{
				purchaseId = Int32.Parse(purchaseIdStr);
				Purchase purchase = purchaseService.FindEntity(purchaseId);
				if (purchase.PayingTime != null)
				{
					throw new Break($"Already payed : {purchaseId}", BreakCode.AlreadyPayed);
				}
			}
			catch (Exception e)
			{
				Break brk = e is Break b ? b : new Break("Not a number", BreakCode.Nan);
				await webSocket.Error(brk.Content.ToJson());
				throw brk;
			}
			return purchaseId;
		}

		private async Task HandleWebSocketRequest(HttpContext context, PurchaseService purchaseService)
		{
			NiceWebSocket webSocket = new NiceWebSocket(context);
			int purchaseId = await ReadPurchaseId(webSocket, purchaseService);
		}

		public async Task Invoke(HttpContext context, PurchaseService purchaseService)
		{
			if (context.Request.Path == "/ws")
			{
				if (context.WebSockets.IsWebSocketRequest)
				{
					await HandleWebSocketRequest(context, purchaseService);
				}
				else
				{
					throw new Break("Only for ws protocol", BreakCode.BadRoute, 400);
				}
			}
			else
			{
				await Next(context);
			}
		}
	}
}
