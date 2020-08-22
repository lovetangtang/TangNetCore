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


            #region ���Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                // ��ȡxml�ļ���
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // ��ȡxml�ļ�·��
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // ��ӿ�������ע�ͣ�true��ʾ��ʾ������ע��
                //options.IncludeXmlComments(xmlPath, true);


                #region Jwt
                //����Ȩ��С��
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //��header�����token�����ݵ���̨
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���)ֱ���������������Bearer {token}(ע������֮����һ���ո�) \"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion
            });
            #endregion

            #region ���EFCore
            services.Configure<DBConnectionOption>(Configuration.GetSection("ConnectionStrings"));//ע��������
            var sqlConnection = Configuration.GetConnectionString("WriteConnection");
            services.AddDbContext<ApiDBContent>(option => option.UseSqlServer(sqlConnection));

            //��д����

            //services.AddTransient<DbContext, ApiDBContent>();

            #endregion

            #region ���jwt��֤
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
            #region ����ע���Զ���Service�ӿ�

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

            #region ע��Redis
            // Redis�ͻ���Ҫ����ɵ����� ��Ȼ�ڴ���������������ʱ�� �����redis client�������ͷš���һ����Ҳȷ��api���������ǵ���ģʽ��
            string redisConnect = Configuration.GetConnectionString("redis");
            //var csredis = new CSRedisClient(redisConnect);
            //redis��Ⱥ
            var csredis = new CSRedis.CSRedisClient(null,
              "127.0.0.1:6379,password=123456,defaultDatabase=0",
              "127.0.0.1:6379,password=123456,defaultDatabase=1",
              "127.0.0.1:6379,password=123456,defaultDatabase=2",
              "127.0.0.1:6379,password=123456,defaultDatabase=3");

            RedisHelper.Initialization(csredis);
            services.AddSingleton(csredis);

            services.AddSingleton<IDistributedCache>(new CSRedisCache(new CSRedisClient(redisConnect)));

            // ����Redis����������ʱ6380�˿ڡ�
            services.AddSingleton<IDistributedSessionCache>(new CSRedisSessionCache(new CSRedisClient(redisConnect)));
            services.AddRedisSession();

            services.AddScoped(typeof(RedisCoreHelper));
            #endregion

            #region ����CAP
            services.AddCap(x =>
            {
                x.UseEntityFramework<ApiDBContent>();

                //���ò������
                x.UseDashboard();
                //ʹ��RabbitMQ
                x.UseRabbitMQ(rb =>
                {
                    //rabbitmq��������ַ
                    rb.HostName = "localhost";

                    rb.UserName = "guest";
                    rb.Password = "guest";

                    //ָ��Topic exchange���ƣ���ָ���Ļ�����Ĭ�ϵ�
                    rb.ExchangeName = "cap.text.exchange";
                });

                //���ô���ɹ������������ݿ��б����ʱ�䣨�룩��Ϊ��֤ϵͳ���ܣ����ݻᶨ������
                x.SucceedMessageExpiredAfter = 24 * 3600;

                //����ʧ�����Դ���
                x.FailedRetryCount = 5;
            });
            #endregion


            // �����ʵ��IDistributedCache�����쳣��
            services.AddSession();

            //��Ӻ�̨��������
            services.AddHostedService<BackgroundJob>();

            #region ע�� Quartz������
            //ע�� Quartz������
            services.AddSingleton<QuartzStartup>();
            services.AddTransient<UserInfoSyncjob>();      // ����ʹ��˲ʱ����ע��
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//ע��ISchedulerFactory��ʵ����

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

            // ���Swagger�й��м��
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Demo v1");
            });

            //������֤
            app.UseAuthentication();
            app.UseAuthorization();



            app.UseSession();

            //������ʼҳ

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

            //��ȡǰ��ע���Quartz������
            var quartz = app.ApplicationServices.GetRequiredService<QuartzStartup>();
            appLifetime.ApplicationStarted.Register(() =>
            {
                quartz.Start().Wait(); //��վ�������ִ��
            });

            appLifetime.ApplicationStopped.Register(() =>
            {
                quartz.Stop();  //��վֹͣ���ִ��

            });
        }
    }
}
