using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using DTO;
    using Services;

    public abstract class AppController<T> : ControllerBase where T : DTO
    {
        private Service<T> service;

        public AppController(Service<T> service)
        {
            this.service = service;
        }

        [HttpGet]
        [Route("")]
        public IActionResult FilterOrFindAll(string filter = null)
        {
            try
            {
                return Ok(filter == null ? this.service.FindAll() : this.service.Filter(filter));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpPost]
        [Route("")]
        public IActionResult Save([FromBody] T data)
        {
            try
            {
                T dto = this.service.Save(data);
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
                return Ok(this.service.FindById(id));
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
                this.service.Delete(id);
                return Ok(Utils.Quote("The deletion has been successful"));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, [FromBody] T data)
        {
            try
            {
                return Ok(this.service.Update(id, data));
            }
            catch (AppException e)
            {
                return e.HandleFrom(this);
            }
        }
    }
}
