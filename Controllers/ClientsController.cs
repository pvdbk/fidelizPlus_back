using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Models;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class ClientsController : AppController<Client, ClientDTO>
    {
        public ClientsController(ClientService service) : base(service)
        {
        }

        [HttpGet]
        [Route("{id}/accounts")]
        public IActionResult Accounts(int id, string filter = null)
        {
            try
            {
                return Ok(((ClientService)this.Service).Accounts(id));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{clientId}/accounts/{accountId}")]
        public IActionResult FindAccount(int clientId, int accountId)
        {
            try
            {
                return Ok(((ClientService)this.Service).FindAccount(clientId, accountId));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{clientId}/accounts/{accountId}")]
        public IActionResult UpdateAccount(int clientId, int accountId, int amount)
        {
            try
            {
                return Ok(((ClientService)this.Service).UpdateAccount(clientId, accountId, amount));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{id}/traders")]
        public IActionResult Traders(int id, string filter = null)
        {
            try
            {
                return Ok(((ClientService)this.Service).Traders(id, filter));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{clientId}/traders/{traderId}")]
        public IActionResult MarkTrader(int clientId, int traderId, bool? bookmark)
        {
            try
            {
                return bookmark == null
                    ? (IActionResult)NoContent()
                    : Ok(((ClientService)this.Service).MarkTrader(clientId, traderId, (bool)bookmark));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }
    }
}
