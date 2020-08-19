using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EFCore.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly ILogger<RedisController> _logger;
        public RedisController(ILogger<RedisController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 发布redis消息队列
        /// </summary>
        /// <param name="eqidPairs"></param>
        /// <returns></returns>
        [Route("batch")]
        [HttpPost]
        public async Task BatchPutEqidAndProfileIds([FromBody]List<CmNumberInfo> eqidPairs)
        {
            if (!ModelState.IsValid)
                throw new ArgumentException("Http Body Payload Error.");
            var redisKey = $"{DateTime.Now.ToString("yyyyMMdd")}";
            if (eqidPairs != null && eqidPairs.Any())
                RedisHelper.LPush(redisKey, eqidPairs.ToArray());
            await Task.CompletedTask;
        }
        /// <summary>
        /// 测试redisseesion
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSessionData(string key)
        {
            var msg = HttpContext.Connection.LocalPort.ToString();
            DateTime dateTime = default;
            if (HttpContext.Session.TryGetValue(key, out var value))
                dateTime = JsonSerializer.Deserialize<DateTime>(value);
            else
            {
                dateTime = DateTime.Now;
                HttpContext.Session.Set(key, JsonSerializer.SerializeToUtf8Bytes(dateTime));
            }

            _logger.LogInformation($"本次连接端口{msg},通过Session获得时间值{dateTime}");
            return new JsonResult(dateTime);
        }

        /// <summary>
        /// 测试redis缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetSetDataTest(string key)
        {
            var msg = HttpContext.Connection.LocalPort.ToString();
            DateTime dateTime = default;
            if (RedisHelper.Get(key) != null)
            {
                dateTime = JsonSerializer.Deserialize<DateTime>(RedisHelper.Get(key));
                RedisHelper.Del(key);
            }
            else
            {
                dateTime = DateTime.Now;
                RedisHelper.Set(key, JsonSerializer.SerializeToUtf8Bytes(dateTime));
            }

            _logger.LogInformation($"本次连接端口{msg},通过Session获得时间值{dateTime}");
            return new JsonResult(dateTime);
        }
    }
}