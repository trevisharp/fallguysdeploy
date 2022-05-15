using System;
using System.Linq;
using server.Models;
using System.Threading.Tasks;
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
}