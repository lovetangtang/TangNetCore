using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quartzs
{
    /// <summary>
    /// 后台任务线程
    /// </summary>
    public class BackgroundJob : BackgroundService
    {
        private  CSRedisClient _cSRedisClients;
        private readonly IConfiguration _conf;
        private readonly ILogger _logger;
        public BackgroundJob( CSRedisClient csRedisClients, IConfiguration conf, ILoggerFactory loggerFactory)
        {
            _cSRedisClients = csRedisClients;
            _conf = conf;
            _logger = loggerFactory.CreateLogger(nameof(BackgroundJob));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service starting");
            if (_cSRedisClients == null)
            {
                _cSRedisClients = new CSRedisClient(_conf.GetConnectionString("redis") + ",defaultDatabase=" + 0);
            }
            RedisHelper.Initialization(_cSRedisClients);

            while (!stoppingToken.IsCancellationRequested)
            {
                var key = $"{DateTime.Now.ToString("yyyyMMdd")}";
                var eqidpair = RedisHelper.BRPop(5, key);
                if (eqidpair != null)
                    _logger.LogInformation(eqidpair);
                //await _eqidPairHandler.AcceptEqidParamAsync(JsonConvert.DeserializeObject<EqidPair>(eqidpair));
                // 强烈建议无论如何休眠一段时间，防止突发大流量导致webApp进程CPU满载，自行根据场景设置合理休眠时间
                await Task.Delay(10, stoppingToken);
            }
            _logger.LogInformation("Service stopping");
        }
    }
}
