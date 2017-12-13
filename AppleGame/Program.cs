
using System.IO;
using Microsoft.AspNetCore.Hosting;
using AntDesigner.GameCityBase.boxs;
using AntDesigner.NetCore.GameCity;

namespace AntDesigner.AppleGame
{
    public class Program
    {
  
        public static void Main(string[] args)
        {
            BoxsManager.Degree= "BuilderNormal";
            GameCity.IsColsed = false;
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()//提供了应用程序的入口
                .Build();
            host.Run();
        }
    }   
}
