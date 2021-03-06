using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
	        /*.ConfigureServices((context, services) =>
		{
		    services.Configure<KestrelServerOptions>(
			context.Configuration.GetSection("Kestrel"));
		})*/
                .ConfigureWebHostDefaults(webBuilder =>
                {
		    webBuilder.ConfigureKestrel(serverOptions =>
		    {
		    	serverOptions.ListenAnyIP(5000);
		    });
		    //webBuilder.UseUrls("http://*:5000;http://localhost:5001;https://hostname:5002");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
