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
        public IActionResult Accounts(int id, string filter)
        {
            return Ok(((ClientService)this.Service).Accounts(id, filter));
        }

        [HttpGet]
        [Route("{clientId}/accounts/{accountId}")]
        public IActionResult FindAccount(int clientId, int accountId)
        {
            return Ok(((ClientService)this.Service).FindAccount(clientId, accountId));
        }

        [HttpGet]
        [Route("{clientId}/accounts/{accountId}")]
        public IActionResult UpdateAccount(int clientId, int accountId, int amount)
        {
            return Ok(((ClientService)this.Service).UpdateAccount(clientId, accountId, amount));
        }

        [HttpGet]
        [Route("{id}/traders")]
        public IActionResult Traders(int id, string filter)
        {
            return Ok(((ClientService)this.Service).Traders(id, filter));
        }

        [HttpGet]
        [Route("{clientId}/traders/{traderId}")]
        public IActionResult MarkTrader(int clientId, int traderId, bool? bookmark)
        {
            return bookmark == null
                ? (IActionResult)NoContent()
                : Ok(((ClientService)this.Service).MarkTrader(clientId, traderId, (bool)bookmark));
        }
    }
}
