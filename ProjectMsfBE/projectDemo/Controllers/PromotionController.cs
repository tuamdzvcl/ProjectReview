using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.DTO.Request.Promotion;
using projectDemo.DTO.UpdateRequest.Promotion;
using projectDemo.Service.PromotionService;

namespace projectDemo.Controllers
{
    [Authorize]
    [Route("api/promotion")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpGet("all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] PromotionQuery query)
        {
            var result = await _promotionService.GetAllPromotionsAsync(query);
            return Ok(result);
        }

        // ADMIN ENDPOINTS
        [HttpPost("admin")]
        public async Task<IActionResult> Create(PromotionCreateRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _promotionService.CreatePromotionAsync(request,userId);
            return Ok(result);
        }

        [HttpPut("admin/{id}")]
        public async Task<IActionResult> Update(int id, PromotionUpdateRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            var result = await _promotionService.UpdatePromotionAsync(id, request);
            return Ok(result);
        }

        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            var result = await _promotionService.GetPromotionByIdAsync(id);
            return Ok(result);
        }

        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _promotionService.DeletePromotionAsync(id);
            return Ok(result);
        }

        [HttpPost("admin/import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var result = await _promotionService.ImportPromotionsAsync(file);
            return Ok(result);
        }

        [HttpGet("admin/export")]
        public async Task<IActionResult> Export()
        {
            var bytes = await _promotionService.ExportPromotionsAsync();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Promotions.xlsx");
        }

        [HttpGet("admin/template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var bytes = await _promotionService.DownloadTemplateAsync(userId);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PromotionTemplate.xlsx");
        }
    }
}
