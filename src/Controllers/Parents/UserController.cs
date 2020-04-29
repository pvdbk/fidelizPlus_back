using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using AppDomain;
    using Services;

    public abstract class UserController<TEntity, TPrivate, TPublic, TAccount, TAccountDTO> : ControllerBase
        where TEntity : UserEntity<TAccount>, new()
        where TPrivate : UserDTO<TAccountDTO>, new()
        where TPublic : class, new()
        where TAccount : Account, new()
        where TAccountDTO : AccountDTO, new()
    {
        private UserEntityService<TEntity, TPrivate, TPublic, TAccount, TAccountDTO> Service { get; }

        public UserController(UserEntityService<TEntity, TPrivate, TPublic, TAccount, TAccountDTO> service) : base() =>
            Service = service;

        [HttpGet]
        [Route("")]
        public IActionResult FilterOrFindAll(string filter) =>
            Ok(Service.FilterOrFindAll(filter));

        [HttpPost]
        [Route("")]
        public IActionResult Save([FromBody] TPrivate bodyDTO)
        {
            (TPrivate dto, int id) = Service.CheckSave(bodyDTO);
            return Created($"http://{Request.Host}{Request.Path}/{id}", dto);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult FindById(int id)
            => Ok(Service.FindById(id));

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            Service.Delete(id);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, [FromBody] TPrivate dto)
            => Ok(Service.CheckUpdate(id, dto, HttpContext));

        [HttpGet]
        [Route("{id}/account")]
        public IActionResult GetAccount(int id) =>
            Ok(Service.GetAccountDTO(id));

        [HttpGet]
        [Route("{id}/purchases")]
        public IActionResult Purchases(int id, string filter) =>
            Ok(Service.Purchases(id, filter));

        [HttpGet]
        [Route("connectionId/{connectionId}")]
        public IActionResult FindByConnectionId(string connectionId) =>
            Ok(Service.FindByConnectionId(connectionId));
    }
}
