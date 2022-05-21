using System;
using System.Linq;
using server.Models;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace server.Controllers;

[ApiController]
public class ConfigurationController : ControllerBase
{
    [HttpPost("sendconfig")]
    public async Task<object> sendconfig([FromBody]Configuration newconf)
    {
        try
        {
            var user = await TokenSystem.Test(newconf?.Token);
            if (user == null)
            {
                return new {
                    status = "MT"
                };
            }

            var config = (await ConfigurationPage.Where(p => p.UserId == user.Id)).FirstOrDefault();
            if (config == null)
                config = new ConfigurationPage();
            config.UserId = user.Id;
            config.Token = newconf.Token;

            foreach (var props in typeof(ConfigurationPage).GetProperties())
            {
                if (props.Name == newconf.Config)
                    props.SetValue(config, newconf?.Value.ToString());
            }
            await config.Save();
            
            return new {
                status = "OK"
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

    [HttpGet("getconfig")]
    public async Task<object> getconfig([FromBody]string token)
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
            var page = (await ConfigurationPage.Where(c => c.UserId == user.Id)).FirstOrDefault();
            if (page == null)
            {
                page = new ConfigurationPage();
                page.UserId = user.Id;
            }
            page.Token = token;
            await page.Save();

            List<object> list = new List<object>();
            foreach (var props in typeof(ConfigurationPage).GetProperties())
            {
                if (props.GetCustomAttribute<ConfigAttribute>() != null)
                {
                    var obj = props.GetValue(page);
                    if (obj != null)
                    {
                        list.Add(new object[2] {
                            props.Name, obj
                        });
                    }
                }
            }

            return new {
                status = "OK",
                content = list
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
}