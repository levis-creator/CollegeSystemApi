using Microsoft.AspNetCore.Mvc;
using CollegeSystemApi.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeSystemApi.DTOs.Response;

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

        /// <summary>
        /// Retrieves all records of type <typeparamref name="T"/>.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ResponseDto<List<T>>>> GetAll()
        {
            var result = await _genericService.GetAllAsync();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Retrieves a record by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseDto<T>>> GetById(int id)
        {
            var result = await _genericService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Adds a new record.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ResponseDto<T>>> Add([FromBody] T entity)
        {
            if (entity == null)
                return BadRequest(ResponseDto<T>.ErrorResultForData(400, "Entity cannot be null."));

            var result = await _genericService.AddAsync(entity);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Updates an existing record.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResponseDto<T>>> Update(int id, [FromBody] T entity)
        {
            if (entity == null)
                return BadRequest(ResponseDto<T>.ErrorResultForData(400, "Entity cannot be null."));

            var result = await _genericService.Update(entity);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Deletes a record by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponseDto>> Delete(int id)
        {
            var entityResult = await _genericService.GetByIdAsync(id);
            if (entityResult.TypedData == null)
                return NotFound(ResponseDto.ErrorResult(404, "Entity not found."));

            var result = await _genericService.Delete(entityResult.TypedData);
            return StatusCode(result.StatusCode, result);
        }
    }
}
