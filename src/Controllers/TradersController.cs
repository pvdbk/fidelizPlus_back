using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace fidelizPlus_back.Controllers
{
    using AppDomain;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class TradersController : UserController<Trader, PrivateTrader, PublicTrader, TraderAccount, TraderAccountDTO>
    {
        private TraderService TraderService { get; }
        private MultiService MultiService { get; }
		private LogoService LogoService { get; }

		public TradersController(
			TraderService traderService,
			MultiService multiService,
			LogoService logoService
		) : base(traderService)
        {
            TraderService = traderService;
            MultiService = multiService;
			LogoService = logoService;
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

		[HttpPost]
		[Route("{traderId}/logo")]
		public async Task<IActionResult> Save(int traderId, IFormFile formFile)
		{
			await LogoService.Save(traderId, formFile);
			return NoContent();
		}

		[HttpGet]
		[Route("{traderId}/logo")]
		public IActionResult Get(int traderId, bool confirm = false)
		{
			if (!confirm)
			{
				LogoService.Get(traderId, LogoService.LOGO_PREFIX);
			}
			LogoService.Confirm(traderId);
			return NoContent();
		}

		[HttpGet]
		[Route("{traderId}/logo/tmp")]
		public void GetTmp(int traderId) => LogoService.Get(traderId, LogoService.TMP_PREFIX);
	}
}
