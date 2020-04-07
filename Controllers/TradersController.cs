using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Models;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class TradersController : AppController<Trader, TraderDTO>
    {
        public TradersController(TraderService service) : base(service)
        {
        }

        [HttpGet]
        [Route("{id}/accounts")]
        public IActionResult Accounts(int id, string filter)
        {
            return Ok(((TraderService)this.Service).Accounts(id, filter));
        }

        [HttpGet]
        [Route("{TraderId}/accounts/{accountId}")]
        public IActionResult FindAccount(int traderId, int accountId)
        {
            return Ok(((TraderService)this.Service).FindAccount(traderId, accountId));
        }

        [HttpGet]
        [Route("{id}/clients")]
        public IActionResult Clients(int id, string filter)
        {
            return Ok(((TraderService)this.Service).Clients(id, filter));
        }
    }
}
