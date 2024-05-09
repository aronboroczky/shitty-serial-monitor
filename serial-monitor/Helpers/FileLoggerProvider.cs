using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using serial_monitor.Settings;
using System.IO.Compression;

namespace serial_monitor.Helpers
{
    internal class FileLoggerProvider : ILoggerProvider
    {
        public readonly Logging Options;

        public FileLoggerProvider(IOptions<Logging> loggerSettings)
        {
            Options = loggerSettings?.Value ?? throw new ArgumentNullException(nameof(loggerSettings));

            if (!Directory.Exists(Options.LogDirectory))
            {
                Directory.CreateDirectory(Options.LogDirectory);
            }

            var archiveYearAndMonth = DateTime.Now.AddMonths(-Options.ArchiveTimeMonth).ToString("yyyyMM");
            var filesToArchive = Directory.GetFiles(Options.LogDirectory).ToList().Where(fn => fn.Contains($"log_{archiveYearAndMonth}"));

            if (filesToArchive.Count() < 1)
            {
                return;
            }

            if (!Directory.Exists($"{Options.LogDirectory}\\archive"))
            {
                Directory.CreateDirectory($"{Options.LogDirectory}\\archive");
            }

            if (!Directory.Exists($"{Options.LogDirectory}\\{archiveYearAndMonth}"))
            {
                Directory.CreateDirectory($"{Options.LogDirectory}\\{archiveYearAndMonth}");
            }

            foreach (var oldLog in filesToArchive)
            {
                var fi = new FileInfo(oldLog);
                File.Move(oldLog, $"{Options.LogDirectory}\\{archiveYearAndMonth}\\{fi.Name}");
            }

            ZipFile.CreateFromDirectory($"{Options.LogDirectory}\\{archiveYearAndMonth}", $"{Options.LogDirectory}\\{archiveYearAndMonth}.zip");
            Directory.Delete($"{Options.LogDirectory}\\{archiveYearAndMonth}", true);
            File.Move($"{Options.LogDirectory}\\{archiveYearAndMonth}.zip", $"{Options.LogDirectory}\\archive\\{archiveYearAndMonth}.zip");
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(this);
        }

        public void Dispose()
        {
        }
    }
}
