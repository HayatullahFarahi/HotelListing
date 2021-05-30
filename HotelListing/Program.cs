using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

namespace HotelListing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //adding logs to the project to specific path in the system
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: "c:\\hotel-listings\\logs-.txt",
                    outputTemplate:
                    $"{{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}} [{{Level:u3}}] {{Message:lj}}{{NewLine}}{{Exception}}",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information
                ).CreateLogger();
            try
            {
                Log.Information("Application Is Starting");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application Failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog() // for logging error to the api endpoint
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
