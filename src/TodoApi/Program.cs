using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TodoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseWebRoot("public")
                          .UseStartup<Startup>();
        }
    }
}
