using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using projectDemo.Repository.Reset;

namespace projectDemo.Controllers
{
    [Route("api/dev")]
    [ApiController]
    public class ResetDataController : ControllerBase
    {
        private readonly RestRepository _restRepository;

        public ResetDataController(RestRepository restRepository)
        {
            _restRepository = restRepository;
        }
        [HttpPost("reset")]
        public async Task<IActionResult> ResetDb()
        {
            await _restRepository.RestData();
            return Ok(new
            {
                message = "Mầy là thằng nào "
            });
        }
    }
}
