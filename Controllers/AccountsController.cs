using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class AccountsController : AppController<AccountDTO>
    {
        public AccountsController(Service<AccountDTO> service) : base(service)
        {
        }
    }
}
