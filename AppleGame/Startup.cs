#define useMysql
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using WxPayAPI;
using Microsoft.AspNetCore.Mvc.Razor;
using AntDesigner.GameCityBase.EF;
using AntDesigner.GameCityBase;
using AntDesigner.GameCityBase.interFace;
using GameCitys.GamCityBase;
using System.Net.WebSockets;
using System.Threading;
using GameCitys.DomainService;

namespace AntDesigner.AppleGame
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)

                //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
            LoadAppsettings();

        }

        private void LoadAppsettings()
        {

           Connection= Configuration.GetConnectionString("mysqlConnectionStr");
            WxPayConfig.SiteName = Configuration.GetSection("Wx:SiteName").Value;
            JsApiPay.body = Configuration.GetSection("Wx:body").Value;
            WxPayConfig.APPID = Configuration.GetSection("Wx:APPID").Value;
            WxPayConfig.MCHID = Configuration.GetSection("Wx:MCHID").Value;
            WxPayConfig.KEY = Configuration.GetSection("Wx:KEY").Value;
            WxPayConfig.APPSECRET = Configuration.GetSection("Wx:APPSECRET").Value;
            WxPayConfig.SSLCERT_PATH = Configuration.GetSection("Wx:SSLCERT_PATH").Value;
            WxPayConfig.SSLCERT_PASSWORD = Configuration.GetSection("Wx:SSLCERT_PATH").Value;
            WxPayConfig.IP = Configuration.GetSection("Wx:IP").Value;
            WxPayConfig.NOTIFY_URL = WxPayConfig.SiteName + "/weixinPay/getPayResult/";
            WxPayConfig.certName = Configuration.GetSection("Wx:certName").Value;

        }

        public IConfigurationRoot Configuration { get; set; }
        public static string Connection { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        //用来配置用于应用程序内的服务 ,ASP.NET MVC 中的某些功能，需要从 ConfigureServices 中请求某些服务，而这些服务需要在接入请求管道之前先被加入 ConfigureServices 中。
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            IServiceCollection newServices = new ServiceCollection();
               foreach (ServiceDescriptor service in services)
                   {
                       newServices.Add(service);
                   }
            // Add framework services.
            newServices.Configure<IISOptions>(options => options.AutomaticAuthentication = true);
            newServices.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Clear();
                options.AreaViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Games/{2}/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Games/{2}/Views/Shared/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });
            newServices.AddDbContext<DataContext>(options => options.UseMySql(Connection), ServiceLifetime.Scoped);
            newServices.AddApplicationInsightsTelemetry(Configuration);
            newServices.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));
            newServices.AddScoped<ILoginGame, LoginByWeixin>();
            newServices.AddScoped<IStorehouse, StorehouseEF>();
            newServices.AddScoped<IPayService, PayServiceWeixin>();
            newServices.AddScoped<INoticeService, NoticeService>();
            newServices.AddScoped<IPlayerService, PlayerService>();
            newServices.AddScoped<IMessageService, MessageService>();
            newServices.AddScoped<IGameCityService, GameCityService>();
            newServices.AddAuthorization(options => //注册验证条目
            {
                options.AddPolicy("RequireRole", policy => policy.RequireAuthenticatedUser());
            });
            newServices.AddMvc(options =>
            { //注册全局验证
                options.Filters.Add(new AuthorizeFilter("RequireRole"));
            });
          
          var newServiceProvicer=newServices.BuildServiceProvider();
          return newServiceProvicer;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //用于指定 ASP.NET 应用程序将如何响应每一个 HTTP 请求
        // Configure 方法必须接受一个 IApplicationBuilder 参数。一些额外服务，比如 IHostingEnvironment 或 ILoggerFactory 也可以被指定，如果在它们可用情况下，这些服务将会被服务器 注入 进来
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            //app.UseExceptionHandler(WxPayConfig.SiteName);//出现错误重定向到主页
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();
            app.UseSession();
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "MyCookieMiddlewareInstance",
                LoginPath = new PathString("/Game/Index/"),
                AccessDeniedPath = new PathString("/Game/Index/"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });
            app.Map("/ws", appBuilder =>
            {
                var webSocketOptions = new WebSocketOptions()
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(60),
                    ReceiveBufferSize = 4 * 1024,

                };
                appBuilder.UseWebSockets(webSocketOptions);
                appBuilder.Run( context =>
                {
                    WebSocket websocket=null;
                    if (context.WebSockets.IsWebSocketRequest)
                    {

                        websocket = context.WebSockets.AcceptWebSocketAsync().Result;
                        var playerId = context.Session.GetInt32("playerId");
                        ClientWebsocketsManager.Add(playerId, websocket);
                    }

                    while (websocket!=null && websocket.State==WebSocketState.Open)
                    { Thread.Sleep(60000); };
                    return null;
                });

            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Game}/{action=Index}/{id?}");
            });
        }

       
    }
 

}
