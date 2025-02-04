using Microsoft.AspNetCore.Mvc;

namespace HNG_Tasks.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInfo()
        {
            return Ok(new
            {
                email = "yasiroyebo@gmail.com",
                currentDateTime = DateTime.UtcNow.ToString("o"),
                githubUrl = "https://github.com/YastecHub/HNG_Tasks"
            });
        }
    }
}
