using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Infrastructure.AutofacModule
{
    public class IOCApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var ServicesDllFile = Path.Combine(basePath, "Application.dll");//获取注入项目绝对路径
            var assemblysServices = Assembly.LoadFile(ServicesDllFile);//直接采用加载文件的方法
            builder.RegisterAssemblyTypes(assemblysServices).Where(type => !type.IsInterface && !type.IsSealed && !type.IsAbstract).InstancePerLifetimeScope().InstancePerLifetimeScope();
        }
    }
}
