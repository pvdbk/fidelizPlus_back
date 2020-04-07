using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Models;
    using Services;

    public abstract class AppController<TEntity, TDTO> : ControllerBase where TEntity : Entity where TDTO : DTO
    {
        public CrudService<TEntity, TDTO> Service { get; }

        public AppController(CrudService<TEntity, TDTO> service)
        {
            this.Service = service;
        }

        [HttpGet]
        [Route("")]
        public IActionResult FilterOrFindAll(string filter)
        {
            return Ok(this.Service.FilterOrFindAll(filter));
        }

        [HttpPost]
        [Route("")]
        public IActionResult Save([FromBody] TDTO body)
        {
            TDTO dto = this.Service.Save(body);
            return Created($"http://{this.Request.Host}{this.Request.Path}/{dto.Id}", dto);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult FindById(int id)
        {
            return Ok(this.Service.FindById(id));
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            this.Service.Delete(id);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, [FromBody] TDTO dto)
        {
            return Ok(this.Service.Update(id, dto));
        }
    }
}
