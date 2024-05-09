using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using serial_monitor.Helpers;
using serial_monitor.Services;
using serial_monitor.Settings;

namespace serial_monitor
{
    internal class Program
    {
        public IConfiguration Configuration { get; }

        static async Task Main(string[] args)
        {
            // create service collection
            var services = new ServiceCollection();

            if (services == null)
            {
                Environment.Exit(-1);
            }

            ConfigureServices(services);

            // create service provider
            var serviceProvider = services.BuildServiceProvider();

            // entry to run app
            await serviceProvider.GetService<App>().Run(args);
        }


        private static void ConfigureServices(IServiceCollection services)
        {
            // build config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            services.AddLogging(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss.fff ";
                });
                builder.AddDebug();
                builder.AddFileLogger(options =>
                {
                    configuration.GetSection(nameof(Logging)).Bind(options);
                });
            });

            services.Configure<SerialPortSettings>(configuration
                .GetSection(nameof(SerialPortSettings)));


            // add app
            services.AddSingleton<ISerialDataHandler, SerialDataHandler>();

            services.AddTransient<App>();
        }
    }
}