using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDistributedCache _distributedCache;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("GetDistributeCacheData")]
        public IEnumerable<WeatherForecast> GetDistributeCacheData(string key)
        {
            var cache = _distributedCache.Get(key);
            if (cache == null)
            {
                // 这里应该上锁,否则同时会有多个请求,单机测试无所谓。
                // 模拟获取数据
                Thread.Sleep(1000);
                var rng = new Random();
                var list = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();

                //放到缓存。
                _distributedCache.Set(key, JsonSerializer.SerializeToUtf8Bytes(list));
                return list;
            }
            else
            {
                return JsonSerializer.Deserialize<WeatherForecast[]>(cache);
            }
        }


        [HttpGet]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.LogWarning("fdffdf");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
