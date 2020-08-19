using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Logging;

namespace Cache.Redis
{
    /// <summary>
    /// 自定义RedisSession缓存。
    /// </summary>
    public class DistributedRedisSessionStore : DistributedSessionStore
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="cache">自定义RedisSession缓存。</param>
        /// <param name="loggerFactory">日志工厂。</param>
        public DistributedRedisSessionStore(IDistributedSessionCache cache, ILoggerFactory loggerFactory) : base(cache, loggerFactory)
        {
        }
    }
}
