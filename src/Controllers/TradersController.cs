using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using AppDomain;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class TradersController : AppController<Trader, PrivateTrader, PublicTrader, TraderAccount, TraderAccountDTO>
    {
        private TraderService TraderService { get; }
        private MultiService MultiService { get; }

        public TradersController(TraderService traderService, MultiService multiService) : base(traderService)
        {
            TraderService = traderService;
            MultiService = multiService;
        }

        [HttpGet]
        [Route("{id}/clients")]
        public IActionResult Clients(int id, string filter)
            => Ok(MultiService.ClientsForTrader(id, filter));


        [HttpPost]
        [Route("{traderId}/purchases/clients/{clientId}")]
        public IActionResult Save(int clientId, int traderId, decimal amount)
        {
            (PurchaseDTO dto, int id) = MultiService.SavePurchase(clientId, traderId, amount);
            return Created($"http://{Request.Host}{Request.Path}/{id}", dto);
        }

        [HttpDelete]
        [Route("{traderId}/purchases/{purchaseId}")]
        public IActionResult DeletePurchase(int traderId, int purchaseId)
        {
            TraderService.DeletePurchase(traderId, purchaseId);
            return NoContent();
        }
    }
}
