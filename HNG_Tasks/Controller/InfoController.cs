using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HNG_Tasks.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetInfo()
        {
            var response = new
            {
                email = "yasiroyebo@gmail.com",
                currentDateTime = DateTime.UtcNow.ToString("o"),
                githubUrl = "https://github.com/YastecHub/HNG_Tasks"
            };
            return Ok(response);
        }
    }
}
