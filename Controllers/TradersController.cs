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
        public IActionResult Accounts(int id, string filter = null)
        {
            try
            {
                return Ok(((TraderService)this.Service).Accounts(id));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{TraderId}/accounts/{accountId}")]
        public IActionResult FindAccount(int traderId, int accountId)
        {
            try
            {
                return Ok(((TraderService)this.Service).FindAccount(traderId, accountId));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{id}/clients")]
        public IActionResult Clients(int id, string filter = null)
        {
            try
            {
                return Ok(((TraderService)this.Service).Clients(id, filter));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }
    }
}
