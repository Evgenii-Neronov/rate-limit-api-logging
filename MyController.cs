using Microsoft.AspNetCore.Mvc;

namespace MyWebApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyController : ControllerBase
    {
        [HttpGet("/hello")]
        public IActionResult Hello([FromQuery] string name)
        {
            return Ok($"Hello {name}");
        }

        [HttpPost("/sum")]
        public IActionResult Sum([FromBody] SumRequest request)
        {
            return Ok(request.A + request.B);
        }
    }

    public record SumRequest(int A, int B);


}
