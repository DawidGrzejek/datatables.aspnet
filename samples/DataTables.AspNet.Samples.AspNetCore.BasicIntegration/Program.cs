using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DataTables.AspNet.Samples.AspNetCore.BasicIntegration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build();

            host.Run();
        }
    }
}
