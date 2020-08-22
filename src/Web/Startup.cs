using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application;
using Domain.Authenticate;
using Infrastructure;
using Quartzs;
using Quartz.Spi;
using Quartz;
using Quartz.Impl;
using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Cache.Redis;
using EFCore.Models.Models;
using Swashbuckle.AspNetCore.Filters;
using Autofac;
using Infrastructure.AutofacModule;
using Autofac.Extensions.DependencyInjection;
using Web.Controllers;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();


            #region 添加Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // 获取xml文件名
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // 获取xml文件路径
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 添加控制器层注释，true表示显示控制器注释
                //options.IncludeXmlComments(xmlPath, true);


                #region Jwt
                //开启权限小锁
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //在header中添加token，传递到后台
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格) \"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion
            });
            #endregion

            #region 添加EFCore
            services.Configure<DBConnectionOption>(Configuration.GetSection("ConnectionStrings"));//注入多个链接
            var sqlConnection = Configuration.GetConnectionString("WriteConnection");
            services.AddDbContext<ApiDBContent>(option => option.UseSqlServer(sqlConnection));

            //读写分离

            //services.AddTransient<DbContext, ApiDBContent>();

            #endregion

            #region 添加jwt认证
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                    ValidIssuer = token.Issuer,
                    ValidAudience = token.Audience,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            #endregion
            services.Configure<WebSettings>(Configuration.GetSection("WebSettings"));
            #region 依赖注入自定义Service接口

            //services.Scan(scan =>
            //{
            //    Assembly assemblysServices = Assembly.Load("Application");
            //    scan.FromAssemblies(assemblysServices)
            //    .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase)))
            //           .AsImplementedInterfaces()
            //    .AsSelf()
            //    .WithScopedLifetime();
            //});

            //services.AddDataService();
            //services.AddScoped<IAuthenticateService, AuthenticateService>();
            //services.AddScoped<IUserService, UserService>();
            #endregion

            #region 注入Redis
            // Redis客户端要定义成单例， 不然在大流量并发收数的时候， 会造成redis client来不及释放。另一方面也确认api控制器不是单例模式，
            string redisConnect = Configuration.GetConnectionString("redis");
            //var csredis = new CSRedisClient(redisConnect);
            //redis集群
            var csredis = new CSRedis.CSRedisClient(null,
              "127.0.0.1:6379,password=123456,defaultDatabase=0",
              "127.0.0.1:6379,password=123456,defaultDatabase=1",
              "127.0.0.1:6379,password=123456,defaultDatabase=2",
              "127.0.0.1:6379,password=123456,defaultDatabase=3");

            RedisHelper.Initialization(csredis);
            services.AddSingleton(csredis);

            services.AddSingleton<IDistributedCache>(new CSRedisCache(new CSRedisClient(redisConnect)));

            // 连接Redis的容器，此时6380端口。
            services.AddSingleton<IDistributedSessionCache>(new CSRedisSessionCache(new CSRedisClient(redisConnect)));
            services.AddRedisSession();

            services.AddScoped(typeof(RedisCoreHelper));
            #endregion

            #region 配置CAP
            services.AddCap(x =>
            {
                x.UseEntityFramework<ApiDBContent>();

                //启用操作面板
                x.UseDashboard();
                //使用RabbitMQ
                x.UseRabbitMQ(rb =>
                {
                    //rabbitmq服务器地址
                    rb.HostName = "localhost";

                    rb.UserName = "guest";
                    rb.Password = "guest";

                    //指定Topic exchange名称，不指定的话会用默认的
                    rb.ExchangeName = "cap.text.exchange";
                });

                //设置处理成功的数据在数据库中保存的时间（秒），为保证系统新能，数据会定期清理。
                x.SucceedMessageExpiredAfter = 24 * 3600;

                //设置失败重试次数
                x.FailedRetryCount = 5;
            });
            #endregion


            // 如果不实现IDistributedCache将会异常。
            services.AddSession();

            //添加后台运行任务
            services.AddHostedService<BackgroundJob>();

            #region 注入 Quartz调度类
            //注入 Quartz调度类
            services.AddSingleton<QuartzStartup>();
            services.AddTransient<UserInfoSyncjob>();      // 这里使用瞬时依赖注入
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//注册ISchedulerFactory的实例。

            services.AddSingleton<IJobFactory, IOCJobFactory>();
            #endregion
        }

        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            //containerBuilder.RegisterType<UserService>().As<IUserService>();
            containerBuilder.RegisterModule<IOCApplicationModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Microsoft.Extensions.Hosting.IApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // 添加Swagger有关中间件
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
            });

            //启用验证
            app.UseAuthentication();
            app.UseAuthorization();



            app.UseSession();

            //设置启始页

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("htmlpage.html");
            app.UseDefaultFiles(defaultFilesOptions);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //获取前面注入的Quartz调度类
            var quartz = app.ApplicationServices.GetRequiredService<QuartzStartup>();
            appLifetime.ApplicationStarted.Register(() =>
            {
                quartz.Start().Wait(); //网站启动完成执行
            });

            appLifetime.ApplicationStopped.Register(() =>
            {
                quartz.Stop();  //网站停止完成执行

            });
        }
    }
}
