using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://*:5101");
                }).UseSerilog((context, configuration) =>
                {
                    var sinkOpts = new SinkOptions();
                    sinkOpts.TableName = "Logs";
                    sinkOpts.AutoCreateSqlTable = true;
                    //var columnOpts = new ColumnOptions();
                    //columnOpts.Store.Remove(StandardColumn.Properties);
                    //columnOpts.Store.Add(StandardColumn.LogEvent);
                    //columnOpts.LogEvent.DataLength = 2048;
                    //columnOpts.TimeStamp.NonClusteredIndex = true;
                    configuration
                        .MinimumLevel.Information()
                        // 日志调用类命名空间如果以 Microsoft 开头，覆盖日志输出最小级别为 Information
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                        .Enrich.FromLogContext()
                        // 配置日志输出到控制台
                        .WriteTo.Console()
                        // 配置日志输出到文件，文件输出到当前项目的 logs 目录下
                        // 日记的生成周期为每天
                        .WriteTo.File(Path.Combine("logs", @"log.txt"), rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                        .WriteTo.MSSqlServer(
        connectionString: "Data Source=.\\SQL2019; Database=Test; User ID=sa; Password=123456; MultipleActiveResultSets=True",
        sinkOptions: sinkOpts);

                }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
