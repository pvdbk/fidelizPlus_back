using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class TradersController : AppController<TraderDTO>
    {
        public TradersController(Service<TraderDTO> service) : base(service)
        {
        }
    }
}
