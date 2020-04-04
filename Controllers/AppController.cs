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
            try
            {
                return Ok(this.Service.FilterOrFindAll(filter));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult Save([FromBody] TDTO body)
        {
            try
            {
                TDTO dto = this.Service.Save(body);
                return Created($"http://{this.Request.Host}{this.Request.Path}/{dto.Id}", dto);
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult FindById(int id)
        {
            try
            {
                return Ok(this.Service.FindById(id));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                this.Service.Delete(id);
                return NoContent();
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, [FromBody] TDTO dto)
        {
            try
            {
                return Ok(this.Service.Update(id, dto));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }
    }
}
