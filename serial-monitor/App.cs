using Microsoft.Extensions.Logging;
using serial_monitor.Services;

namespace serial_monitor
{
    internal class App
    {
        private readonly ILogger<App> _logger;
        private readonly ISerialDataHandler _serialDataHandler;

        public App(ILogger<App> logger, ISerialDataHandler serialDataHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _serialDataHandler = serialDataHandler ?? throw new ArgumentNullException();
            _serialDataHandler.SerialDataReceived += _serialDataHandler_SerialDataReceived;
            _serialDataHandler.SerialConnectionChanged += _serialDataHandler_SerialConnectionChanged;


            _logger.LogInformation("Starting serial monitor - to quit type \"quit\" and hit enter");
        }

        public async Task Run(string[] args)
        {
            while (true)
            {
                string input = await Console.In.ReadLineAsync() ?? "";
                if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }
        }

        private void _serialDataHandler_SerialConnectionChanged(object? sender, Models.SerialConnectionEventArgs e)
        {
            _logger.LogInformation($"Serial connection\' state changed to: {e.ConnectionState}");
        }

        private void _serialDataHandler_SerialDataReceived(object? sender, Models.SerialDataReceivedEventArgs e)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy.MM.dd - HH:mm:ss.fff")} - {e.Data}");
            //_logger.LogInformation(e.Data); -- Too much rubbish
        }
    }
}
