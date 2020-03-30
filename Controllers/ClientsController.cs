using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Services;

    [Route("[controller]")]
    [ApiController]
    public class ClientsController : AppController<ClientDTO>
    {
        public ClientsController(Service<ClientDTO> service) : base(service)
        {
        }
    }
}
