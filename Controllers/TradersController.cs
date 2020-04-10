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
        private ClientAndTraderService BothService { get; }

        public TradersController(TraderService traderService, ClientAndTraderService bothService) : base(traderService)
        {
            TraderService = traderService;
            BothService = bothService;
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
            return Ok(BothService.ClientsForTrader(id, filter));
        }
    }
}
