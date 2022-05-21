using System;
using server.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace server.Controllers;

[ApiController]
public class NotificationController : ControllerBase
{
    [HttpPost("sendnotif")]
    public async Task<object> sendnotif([FromBody]Notification not)
    {
        try
        {
            var user = await TokenSystem.Test(not.Token ?? "");
            if (user == null)
            {
                return new {
                    status = "MT"
                };
            }
            not.UserId = user.Id;
            not.Moment = DateTime.Now;
            await not.Save();
            return new {
                status = "OK",
            };
        }
        catch (Exception e)
        {
            return new {
                status = "ER",
                error = e.ToString()
            };
        }
    }
    
    [HttpGet("getnotif")]
    public async Task<object> getnotif([FromBody]string token)
    {
        try
        {
            var user = await TokenSystem.Test(token);
            if (user == null)
            {
                return new {
                    status = "MT"
                };
            }
            return await Notification.Where(n => n.UserId == user.Id);
        }
        catch (Exception e)
        {
            return new {
                status = "ER",
                error = e.ToString()
            };
        }
    }
}