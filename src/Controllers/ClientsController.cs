using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
	using AppDomain;
	using Services;

	[Route("[controller]")]
	[ApiController]
	public class ClientsController : UserController<Client, PrivateClient, PublicClient, ClientAccount, ClientAccountDTO>
	{
		private ClientService ClientService { get; }
		private PurchaseService PurchaseService { get; }
		private MultiService BothService { get; }

		public ClientsController(
			ClientService clientService,
			PurchaseService purchaseService,
			MultiService bothService
		) : base(clientService)
		{
			ClientService = clientService;
			PurchaseService = purchaseService;
			BothService = bothService;
		}

		[HttpGet]
		[Route("{id}/traders")]
		public IActionResult GetTraders(int id, string filter) =>
			Ok(BothService.TradersForClient(id, filter));

		[HttpGet]
		[Route("{clientId}/traders/{traderId}")]
		public IActionResult MarkTrader(int clientId, int traderId, bool? bookmark) =>
			Ok(BothService.MarkTrader(clientId, traderId, bookmark));

		[HttpGet]
		[Route("{clientId}/purchases/{purchaseId}")]
		public IActionResult Pay(int clientId, int purchaseId)
			=> Ok(BothService.Pay(clientId, purchaseId));
	}
}
