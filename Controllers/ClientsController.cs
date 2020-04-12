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
        private MultiService BothService { get; }

        public ClientsController(
            ClientService clientService,
            MultiService bothService
        ) : base(clientService)
        {
            ClientService = clientService;
            BothService = bothService;
        }

        [HttpGet]
        [Route("{id}/account")]
        public IActionResult GetAccount(int id)
        {
            return Ok(ClientService.GetAccount(id));
        }

        [HttpGet]
        [Route("{id}/traders")]
        public IActionResult Traders(int id, string filter)
        {
            return Ok(BothService.TradersForClient(id, filter));
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
