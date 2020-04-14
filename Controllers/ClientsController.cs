using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using AppDomain;
    using DTO;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class ClientsController : AppController<Client, ClientDTO>
    {
        private ClientService ClientService { get; }
        private RelatedToBothService<Purchase, PurchaseDTO> PurchaseService { get; }
        private MultiService BothService { get; }

        public ClientsController(
            ClientService clientService,
            RelatedToBothService<Purchase, PurchaseDTO> purchaseService,
            MultiService bothService
        ) : base(clientService)
        {
            ClientService = clientService;
            PurchaseService = purchaseService;
            BothService = bothService;
        }

        [HttpGet]
        [Route("{id}/account")]
        public IActionResult GetAccount(int id)
        {
            return Ok(ClientService.GetAccountDTO(id));
        }

        [HttpGet]
        [Route("{id}/purchases")]
        public IActionResult Purchases(int id, string filter)
        {
            return Ok(ClientService.Purchases(id, FilterParamToTree(filter)));
        }

        [HttpGet]
        [Route("{id}/traders")]
        public IActionResult Traders(int id, string filter)
        {
            return Ok(BothService.TradersForClient(id, FilterParamToTree(filter)));
        }

        [HttpGet]
        [Route("{clientId}/traders/{traderId}")]
        public IActionResult MarkTrader(int clientId, int traderId, bool? bookmark)
        {
            return Ok(BothService.MarkTrader(clientId, traderId, bookmark));
        }

        [HttpGet]
        [Route("{clientId}/purchases/{purchaseId}")]
        public IActionResult Pay(int clientId, int purchaseId)
        {
            return Ok(BothService.Pay(clientId, purchaseId));
        }
    }
}
