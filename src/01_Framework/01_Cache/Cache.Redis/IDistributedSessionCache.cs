using Microsoft.Extensions.Caching.Distributed;

namespace Cache.Redis
{
    /// <summary>
    /// 继承于分布式缓存接口。
    /// </summary>
    public interface IDistributedSessionCache : IDistributedCache
    {
    }
}
