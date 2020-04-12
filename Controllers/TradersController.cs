using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using AppDomain;
    using DTO;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class TradersController : AppController<Trader, TraderDTO>
    {
        private TraderService TraderService { get; }
        private MultiService MultiService { get; }

        public TradersController(TraderService traderService, MultiService multiService) : base(traderService)
        {
            TraderService = traderService;
            MultiService = multiService;
        }

        [HttpGet]
        [Route("{id}/account")]
        public IActionResult Accounts(int id)
        {
            return Ok(TraderService.GetAccount(id));
        }

        [HttpGet]
        [Route("{id}/clients")]
        public IActionResult Clients(int id, string filter)
        {
            return Ok(MultiService.ClientsForTrader(id, filter));
        }

        [HttpPost]
        [Route("{traderId}/purchases/clients/{clientId}")]
        public IActionResult Save(int clientId, int traderId, decimal amount)
        {
            (PurchaseDTO dto, int id) = MultiService.SavePurchase(clientId, traderId, amount);
            return Created($"http://{Request.Host}{Request.Path}/{id}", dto);
        }
    }
}
