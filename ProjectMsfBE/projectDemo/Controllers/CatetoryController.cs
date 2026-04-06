using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request;
using projectDemo.Service.CatetoryService;

namespace projectDemo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/catetory")]
    public class CatetoryController : ControllerBase
    {
        private readonly ICatetoryService _catetoryService;

        public CatetoryController(ICatetoryService catetoryService)
        {
            _catetoryService = catetoryService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CatetoryResquest request)
        {
            var result = await _catetoryService.Create(request);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CatetoryResquest request)
        {
            var result = await _catetoryService.Update(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _catetoryService.Delete(id);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _catetoryService.Getbyid(id);
            return Ok(result);
        }

        [HttpGet("list-event")]
        public async Task<IActionResult> GetCatetoryListEvent(
            [FromQuery] int pageSize ,
            [FromQuery] int pageIndex ,
            [FromQuery] string? key )
        {
            var result = await _catetoryService.GetCatetoryListEvent(pageSize, pageIndex, key);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetCatetory()
          {
            var result = await _catetoryService.GetCatetory();
            return Ok(result);
        }
    }
}

