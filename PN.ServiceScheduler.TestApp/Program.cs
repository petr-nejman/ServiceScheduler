using PN.ServiceScheduler.Extensions;
using PN.ServiceScheduler.TestApp.Services;
using Serilog;
using System;

namespace PN.ServiceScheduler.TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(builder.Configuration)
                        .Enrich.FromLogContext();
                });

                builder.Services.AddServiceScheduler(conf =>
                {
                    conf.AddScopedJob<ScopedService>("Scoped task").Every(TimeSpan.FromSeconds(10));
                    conf.AddSingletonJob<LongRunningService>("Long task at time").EveryDayAt(new List<TimeOnly>() {
                        TimeOnly.FromDateTime(DateTime.UtcNow.AddSeconds(10)),
                        TimeOnly.FromDateTime(DateTime.UtcNow.AddSeconds(20))
                    });
                    conf.AddSingletonJob<LongRunningService>("Long task").OnceAfter(TimeSpan.FromSeconds(15));
                });

                var app = builder.Build();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "App startup error");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
