using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cache.Redis
{
    public class RedisCoreHelper
    {
        private readonly CSRedisClient redisManger = null;
        private readonly ILogger Log;
        private readonly IConfiguration _conf;
        private CSRedisClient GetClient()
        {
            return redisManger;
        }
        public RedisCoreHelper( ILoggerFactory loggerFactory, IConfiguration conf)
        {
            _conf = conf;
            var  redisconfig = conf.GetConnectionString("redis");
            redisManger = new CSRedisClient(redisconfig);      //Redis的连接字符串
            Log = loggerFactory.CreateLogger(nameof(RedisCoreHelper));
        }

        /// <summary>
        /// TradeManageMessage 和 TradeManageMessage:MQ队列
        /// </summary>
        /// <returns></returns>
        public bool EnQeenTradeManageMessage(string value)
        {
            try
            {
                Log.LogInformation("yinzhou--EnQeenTradeManageMessage:" + value);
                //从头部插入
                GetClient().LPush("TradeManageMessage", value);
                GetClient().LPush("TradeManageMessage:MQ", value);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"EnQeenTradeManageMessage:key=TradeManageMessage:MQ,value={value}", e);
                return false;
            }
        }
        /// <summary>
        /// TradeManageMessage 和 TradeManageMessage:MQ队列
        /// </summary>
        /// <returns></returns>
        public bool EnQeenTradeManageMessage<T>(T value)
        {
            try
            {
                //从头部插入
                GetClient().LPush("TradeManageMessage", value);
                GetClient().LPush("TradeManageMessage:MQ", value);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"EnQeenTradeManageMessage:key=TradeManageMessage:MQ,value={value}", e);
                return false;
            }
        }

        public bool EnQueen(string key, string value)
        {
            try
            {
                //从头部插入
                GetClient().LPush(key, value);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"EnQueen:key={key},value={value}", e);
                return false;
            }
        }

        public string DeQueen(string key)
        {
            string result = "";
            try
            {
                //从尾部取值
                result = GetClient().RPop(key);
                return result;
            }
            catch (Exception e)
            {
                Log.LogError($"DeQueen:key={key}", e);
                return result;
            }
        }
        //redis订阅模式
        public void Sub(string key, Action<string> action)
        {
            GetClient().Subscribe((key, msg => action(msg.Body)));
        }

        public string[] DeQueenAll(string key)
        {
            string[] result = { };
            try
            {
                long len = GetClient().LLen(key);

                //取出指定数量数据
                result = GetClient().LRange(key, 0, len - 1);
                //删除指定数据
                bool res = GetClient().LTrim(key, len, -1);

                return result;
            }
            catch (Exception e)
            {
                Log.LogError($"DeQueen:key={key}", e);
                return result;
            }
        }

        public bool EnQueen<T>(string key, T value)
        {
            try
            {
                //从头部插入
                long len = GetClient().LPush(key, value);
                if (len > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                Log.LogError($"EnQueenObj:key={key},value={value}", e);
                return false;
            }
        }

        public T DeQueen<T>(string key)
        {
            T result = default(T);
            try
            {
                //从尾部取值
                result = GetClient().RPop<T>(key);
                return result;
            }
            catch (Exception e)
            {
                Log.LogError($"DeQueen:key={key}", e);
                return result;
            }
        }

        /// <summary>
        /// 设置hash值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetHash(string key, string field, string value)
        {
            try
            {
                GetClient().HSet(key, field, value);
                return true;
            }
            catch (Exception e)
            {
                Log.LogError($"SetHash:key={key},value={value}", e);
                return false;
            }
        }

        /// <summary>
        /// 根据表名，键名，获取hash值
        /// </summary>
        /// <param name="key">表名</param>
        /// <param name="field">键名</param>
        /// <returns></returns>
        public string GetHash(string key, string field)
        {
            string result = "";
            try
            {

                result = GetClient().HGet(key, field);
                return result;
            }
            catch (Exception e)
            {
                Log.LogError($"GetHash:key={key}", e);
                return result;
            }
        }

        /// <summary>
        /// 获取指定key中所有字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetHashAll(string key)
        {
            try
            {

                var result = GetClient().HGetAll(key);
                return result;
            }
            catch (Exception e)
            {
                Log.LogError($"GetHash:key={key}", e);
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// 根据表名，键名，删除hash值
        /// </summary>
        /// <param name="key">表名</param>
        /// <param name="field">键名</param>
        /// <returns></returns>
        public long DeleteHash(string key, string field)
        {
            long result = 0;
            try
            {
                result = GetClient().HDel(key, field);
                return result;
            }
            catch (Exception e)
            {
                Log.LogError($"GetHash:key={key}", e);
                return result;
            }
        }


        //private object deleteCache( Method method, Object[] args)
        //{
        //    object flag = false;

        //    String fieldkey = parseKey(method, args);
        //    try
        //    {
        //        if (fieldkey.equals(""))
        //        {
        //            cacheClient.del(cache.key());
        //        }
        //        else
        //        {
        //            cacheClient.hdel(cache.key(), fieldkey);
        //        }
        //        flag = true;
        //    }
        //    catch (Exception e)
        //    {
        //        //System.out.println(e.getMessage());
        //        flag = false;
        //    }
        //    return flag;
        //}


        /**
         * 获取值field
         * @param key
         * @param method
         * @param args
         * @return
         * @throws Exception
         */
        //        public string parseKey(Method method, Object[] args)
        //        {
        //            string fieldKey = "";
        //            for (int i = 0; i < method.getParameters().length; i++)
        //            {
        //                Parameter p = method.getParameters()[i];
        //                FieldAnnotation f = p.getAnnotation(FieldAnnotation.class);
        //          if(f!=null) {
        //              fieldKey+=args[i].toString()+":";
        //          }else {
        //              FieldOnlyKeyAnnotation fo = p.getAnnotation(FieldOnlyKeyAnnotation.class);
        //              if(fo != null) {
        //                  fieldKey+=args[i].toString();
        //}
        //          }
        //      }

        //      return fieldKey;
        //    }
    }
}
