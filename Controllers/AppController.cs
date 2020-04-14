using Microsoft.AspNetCore.Mvc;

namespace fidelizPlus_back.Controllers
{
    using AppDomain;
    using Services;

    public abstract class AppController<TEntity, TDTO> : ControllerBase
        where TEntity : Entity, new()
        where TDTO : new()
    {
        private CrudService<TEntity, TDTO> Service { get; }

        public AppController(CrudService<TEntity, TDTO> service)
        {
            Service = service;
        }

        public Tree FilterParamToTree(string s)
        {
            Tree ret = null;
            if (s != null)
            {
                try
                {
                    ret = new Tree(s);
                    if (ret.Type != "object")
                    {
                        throw new AppException(null);
                    }
                }
                catch (AppException)
                {
                    throw new AppException("Bad filter parameter", 400);
                }
            }
            return ret;
        }

        [HttpGet]
        [Route("")]
        public IActionResult FilterOrFindAll(string filter)
        {
            return Ok(Service.GetDTOs(FilterParamToTree(filter)));
        }

        [HttpPost]
        [Route("")]
        public IActionResult Save([FromBody] TDTO bodyDTO)
        {
            (TDTO dto, int id) = Service.CheckSave(bodyDTO);
            return Created($"http://{Request.Host}{Request.Path}/{id}", dto);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult FindById(int id)
        {
            return Ok(Service.FindById(id));
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            Service.Delete(id);
            return NoContent();
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, [FromBody] TDTO dto)
        {
            return Ok(Service.CheckUpdate(id, dto));
        }
    }
}
