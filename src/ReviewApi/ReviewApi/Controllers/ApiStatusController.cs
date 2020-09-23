using Microsoft.AspNetCore.Mvc;

namespace ReviewApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiStatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok();
    }
}
