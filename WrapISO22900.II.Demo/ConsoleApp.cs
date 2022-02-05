using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console;
using System;
using System.IO;
using System.Threading;

//Set the UTF-8 worldwide language support:
//Execute intl.cpl
//    Go to [Administrative], then[Change system locale...]

namespace ISO22900.II.Demo
{
    internal class ConsoleApp
    {
        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                //Next two only if need... not just now
                .AddJsonFile($"appsettings.json.{Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Production"}.json", true)
                .AddEnvironmentVariables();
        }

        private static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                BuildConfig(builder);
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Build())
                    // .MinimumLevel.Debug() // <- Set the minimum level
                    .Enrich.FromLogContext()
                    .Enrich.With(new ThreadIdEnricher())
                    //.Enrich.WithProperty("Version", "1.0")
                    .CreateLogger();

                using ( var host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) => { services.AddTransient<IConsoleHeart, ConsoleHeart>(); })
                    .UseSerilog()
                    .Build() )
                {
                    var app = ActivatorUtilities.CreateInstance<ConsoleHeart>(host.Services);
                    app.Run(args);
                }
            }
            catch ( Exception e )
            {
                AnsiConsole.WriteException(e);
                Console.ReadLine();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private class ThreadIdEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                    "ThreadId", Thread.CurrentThread.ManagedThreadId));
            }
        }
    }
}
