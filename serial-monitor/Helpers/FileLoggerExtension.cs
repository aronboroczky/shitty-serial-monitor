using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using serial_monitor.Settings;

namespace serial_monitor.Helpers
{
    internal static class FileLoggerExtension
    {
        public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<Logging> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }
}
