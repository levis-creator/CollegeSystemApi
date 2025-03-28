using Microsoft.AspNetCore.Mvc;
using CollegeSystemApi.Services;
using CollegeSystemApi.DTOs;
using System.Threading.Tasks;
using CollegeSystemApi.Services.Interfaces;

namespace CollegeSystemApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : ControllerBase where T : class
    {
        private readonly IGenericServices<T> _genericService;

        public GenericController(IGenericServices<T> genericService)
        {
            _genericService = genericService;
        }

        // GET: api/Generic
        [HttpGet]
        public async Task<ActionResult<ResponseDto<IEnumerable<T>>>> GetAll()
        {
            var result = await _genericService.GetAllAsync();
            if (result.StatusCode == 200)
                return Ok(result);
            return NotFound(result);
        }

        // GET: api/Generic/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<T>>> GetById(int id)
        {
            var result = await _genericService.GetByIdAsync(id);
            if (result.StatusCode == 200)
                return Ok(result);
            return NotFound(result);
        }

        // POST: api/Generic
        [HttpPost]
        public async Task<ActionResult<ResponseDto<T>>> Add([FromBody] T entity)
        {
            if (entity == null)
                return BadRequest("Entity cannot be null");

            var result = await _genericService.AddAsync(entity);
            if (result.StatusCode == 200)
                return Ok(result);
            return StatusCode(500, result);
        }

        // PUT: api/Generic/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<T>>> Update(int id, [FromBody] T entity)
        {
            if (entity == null)
                return BadRequest("Entity cannot be null");

            var result = await _genericService.Update(entity);
            if (result.StatusCode == 200)
                return Ok(result);
            return StatusCode(500, result);
        }

        // DELETE: api/Generic/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto<T>>> Delete(int id)
        {
            var entity = await _genericService.GetByIdAsync(id);
            if (entity.Data == null)
                return NotFound("Entity not found");

            var result = await _genericService.Delete(entity.Data);
            if (result.StatusCode == 200)
                return Ok(result);
            return StatusCode(500, result);
        }
    }
}