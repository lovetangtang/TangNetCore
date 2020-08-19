using IoC;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure
{
    public static class DataServiceExtension
    {
        public static IServiceCollection AddDataService(this IServiceCollection services)
        {

            #region 依赖注入
            Dictionary<string, Type> dictInterface = new Dictionary<string, Type>();
            Dictionary<string, Type> dictDAL = new Dictionary<string, Type>();

            string dalSuffix = "Service";
            string interfaceSuffix = "Service";

            //List<Type> listTypes = Assembly.GetExecutingAssembly().GetTypes().ToList();//获取当前程序集中的所有类或接口
            List<Type> listTypes = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Application.dll").GetTypes().ToList();
            //获取配置文件中DALFactoryDLL配置项指定的DLL中的外部定义或扩展的子类或接口          
            //string dlls = "Application.dll";// System.Configuration.ConfigurationManager.AppSettings["DALFactoryDLL"].Trim();
            //string[] dllsArray = dlls.ToUpper().Split(';');
            //if (dllsArray.Length > 0)
            //{
            //    Assembly ass = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "/Application.dll");
            //    AssemblyName assname = ass.GetName();
            //    listTypes.AddRange(ass.GetTypes());
            //}

            //对比 接口和具体的接口实现类，把它们分别放到不同的字典集合里
            foreach (Type objType in listTypes)
            {
                string defaultNamespace = objType.Namespace;
                string assName = objType.Name;
                if (string.IsNullOrEmpty(defaultNamespace))
                    continue;

                if (objType.IsInterface && assName.EndsWith(interfaceSuffix) && assName.StartsWith("I"))
                {
                    if (!dictInterface.ContainsKey(objType.FullName))
                        dictInterface.Add(objType.FullName, objType);
                }
                else if (assName.EndsWith(dalSuffix))
                {
                    if (!dictDAL.ContainsKey(objType.FullName))
                        dictDAL.Add(objType.FullName, objType);
                }
            }

            //根据注册的接口和接口实现集合，使用IOC容器进行注册
            foreach (string key in dictInterface.Keys)
            {
                Type interfaceType = dictInterface[key];
                foreach (string dalKey in dictDAL.Keys)
                {
                    Type dalType = dictDAL[dalKey];
                    if (interfaceType.IsAssignableFrom(dalType))//判断DAL是否实现了某接口
                        services.AddScoped(interfaceType, dalType);
                    else
                        services.AddScoped(dalType);
                }
            }
            #endregion
            return services;
        }
    }
}
