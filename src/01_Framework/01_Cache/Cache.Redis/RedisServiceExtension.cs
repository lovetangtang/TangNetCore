using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cache.Redis
{
    public static class RedisServiceExtension
    {
        public static IServiceCollection AddRedisSession(this IServiceCollection services)
        {
            services.TryAddTransient<ISessionStore, DistributedRedisSessionStore>();
            services.AddDataProtection();
            return services;
        }
    }
}
