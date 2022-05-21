using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("test")]
    public string test()
    {
        return "FallGuys is running!";
    }

    [HttpPost("posttest")]
    public string posttest([FromBody]string txt)
    {
        var result = txt.Select(c => int.Parse(c.ToString()))
            .Sum();
        return result.ToString();
    }
}