using System;
using System.Linq;
using server.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace server.Controllers;

[ApiController]
public class DataController : ControllerBase
{
    [HttpPost("senddata")]
    public async Task<object> senddata([FromBody]DataPack pack)
    {
        try
        {
            var user = await TokenSystem.Test(pack.Token);
            if (user == null)
            {
                return new {
                    status = "MT"
                };
            }
            for (int i = 0; i < (pack?.Steps?.Count ?? 0); i++)
            {
                await new Step()
                {
                    Moment = DateTime.Now,
                    UserId = user.Id,
                    Value = pack.Steps[i]
                }.Save();
            }
            for (int i = 0; i < (pack?.Pressure?.Count ?? 0); i += 2)
            {
                await new Pressure()
                {
                    Moment = DateTime.Now,
                    UserId = user.Id,
                    Low = pack.Pressure[i],
                    High = pack.Pressure[i + 1]
                }.Save();
            }
            for (int i = 0; i < (pack?.Livelocation?.Count ?? 0); i += 2)
            {
                await new LiveLocation()
                {
                    Moment = DateTime.Now,
                    UserId = user.Id,
                    Latitude = pack.Livelocation[i],
                    Longitude = pack.Livelocation[i + 1]
                }.Save();
            }
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

    [HttpGet("getlastdaydata")]
    public async Task<object> getlastdaydata([FromBody]string token)
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
            var pressures = await Pressure.Where(dp => dp.UserId == user.Id);
            var steps = await Step.Where(dp => dp.UserId == user.Id);
            var livelocations = await LiveLocation.Where(dp => dp.UserId == user.Id);
            var notifications = await Notification.Where(n => n.UserId == user.Id);

            var livelocation = livelocations.LastOrDefault();
            var pressure = pressures.LastOrDefault();
            var step = steps.Reverse().Take(10).Reverse();
            var notification = notifications.Where(n => (DateTime.Now - (n.Moment ?? DateTime.Now)).Hours < 25);
            
            foreach (var x in pressures)
            {
                x.SendedToUser = true;
                await x.Save();
            }
            foreach (var x in steps)
            {
                x.SendedToUser = true;
                await x.Save();
            }
            foreach (var x in livelocations)
            {
                x.SendedToUser = true;
                await x.Save();
            }
            foreach (var x in notifications)
            {
                x.SendedToUser = true;
                await x.Save();
            }

            var result = new List<object>();

            if (livelocation != null)
                result.Add(tuple("gps", tuple(livelocation?.Latitude ?? 0, livelocation?.Longitude ?? 0)));
            if (pressure != null)
            {
                result.Add(tuple("barometer", tuple("anklet", pressure?.Low ?? 0)));
                result.Add(tuple("barometer", tuple("base", pressure?.Base ?? 0)));
                result.Add(tuple("barometer", tuple("necklace", pressure?.High ?? 0)));
            }
            result.AddRange(step.Select(s => tuple("step", tuple(s.Value))));
            result.AddRange(notification.Select(n => tuple("notification", tuple(n?.Type, n?.Message))));

            return new {
                status = "OK",
                content = result
            };

            List<object> tuple(params object[] data)
            {
                var list = new List<object>();
                list.AddRange(data);
                return list;
            }
        }
        catch (Exception e)
        {
            return new {
                status = "ER",
                error = e.ToString()
            };
        }
    }

    [HttpGet("getnewdata")]
    public async Task<object> getnewdata([FromBody]string token)
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
            var pressures = await Pressure.Where(dp => dp.UserId == user.Id);
            var steps = await Step.Where(dp => dp.UserId == user.Id);
            var livelocations = await LiveLocation.Where(dp => dp.UserId == user.Id);
            var notifications = await Notification.Where(n => n.UserId == user.Id);
            
            pressures = pressures.Where(x =>!(x.SendedToUser ?? true));
            steps = steps.Where(x =>!(x.SendedToUser ?? true));
            livelocations = livelocations.Where(x =>!(x.SendedToUser ?? true));
            notifications = notifications.Where(x =>!(x.SendedToUser ?? true));

            var livelocation = livelocations.LastOrDefault();
            var pressure = pressures.LastOrDefault();
            var step = steps.Reverse().Take(10).Reverse();
            var notification = notifications.Where(n => (DateTime.Now - (n.Moment ?? DateTime.Now)).Hours < 25);

            var result = new List<object>();

            if (livelocation != null)
                result.Add(tuple("gps", tuple(livelocation?.Latitude ?? 0, livelocation?.Longitude ?? 0)));
            if (pressure != null)
            {
                result.Add(tuple("barometer", tuple("anklet", pressure?.Low ?? 0)));
                result.Add(tuple("barometer", tuple("base", pressure?.Base ?? 0)));
                result.Add(tuple("barometer", tuple("necklace", pressure?.High ?? 0)));
            }
            result.AddRange(step.Select(s => tuple("step", tuple(s.Value))));
            result.AddRange(notification.Select(n => tuple("notification", tuple(n?.Type, n?.Message))));
            result.AddRange(step.Select(s => tuple("step", tuple(s.Value))));
            result.AddRange(notification.Select(n => tuple("notification", tuple(n?.Type, n?.Message))));
            
            foreach (var x in pressures)
            {
                x.SendedToUser = true;
                await x.Save();
            }
            foreach (var x in steps)
            {
                x.SendedToUser = true;
                await x.Save();
            }
            foreach (var x in livelocations)
            {
                x.SendedToUser = true;
                await x.Save();
            }
            foreach (var x in notifications)
            {
                x.SendedToUser = true;
                await x.Save();
            }

            return new {
                status = "OK",
                content = result
            };

            List<object> tuple(params object[] data)
            {
                var list = new List<object>();
                list.AddRange(data);
                return list;
            }
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