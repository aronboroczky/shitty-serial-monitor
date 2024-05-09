using serial_monitor.Models.Enums;
using serial_monitor.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using serial_monitor.Settings;

namespace serial_monitor.Services
{
    internal class SerialDataHandler : ISerialDataHandler
    {
        private readonly System.IO.Ports.SerialPort _serialPort;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly ILogger<SerialDataHandler> _logger;
        private readonly SerialPortSettings _settings;

        public event EventHandler<SerialConnectionEventArgs> SerialConnectionChanged;
        public event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;

        public bool IsSerialPortOpenned { get; set; } = false;

        public SerialDataHandler(IOptions<SerialPortSettings> settings, ILogger<SerialDataHandler> logger)
        {
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _serialPort = new System.IO.Ports.SerialPort(_settings.PortName, _settings.BaudRate, _settings.Parity,
                _settings.DataBits, _settings.StopBits);

            _serialPort.DataReceived += _serialPort_DataReceived;

            _cancellationTokenSource = new CancellationTokenSource();

            Open();

            Task.Run(MonitorAndOpenSerialPort, _cancellationTokenSource.Token);
        }

        private void _serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                _logger.LogDebug($"Data received on serial port {_serialPort.PortName}.");
                string data = _serialPort.ReadLine();
                SerialDataReceived?.Invoke(this, new SerialDataReceivedEventArgs(data));
            }
            catch (Exception seriEx)
            {
                _logger.LogError($"Failed to read data from serial port: {seriEx.Message}");
            }
        }

        private async Task MonitorAndOpenSerialPort()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                if (!_serialPort.IsOpen)
                {
                    SerialConnectionChanged?.Invoke(this, new SerialConnectionEventArgs(_serialPort.PortName, SerialConnectionState.Disconnected));
                    Open();
                }

                await Task.Delay(TimeSpan.FromSeconds(1)); // check every second
            }
        }

        public async void Open()
        {
            try
            {
                _serialPort.Open();
                SerialConnectionChanged?.Invoke(this, new SerialConnectionEventArgs(_serialPort.PortName, SerialConnectionState.Connected));
                _logger.LogDebug($"Serial port {_serialPort.PortName} opened.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to open serial port: {ex.Message}");
            }
        }

        public void Close()
        {
            _logger.LogDebug($"Closing serial port {_serialPort.PortName}.");
            _serialPort.Close();
        }

        public void Dispose()
        {
            _serialPort.DataReceived -= _serialPort_DataReceived;
            _cancellationTokenSource.Cancel(); // stop the background task
            _serialPort?.Dispose();
        }

        public bool IsConnected()
        {
            return _serialPort.IsOpen;
        }

        public string GetPort()
        {
            return _serialPort.PortName;
        }

        public void ClearBuffer()
        {
            _logger.LogDebug($"Clearing buffer of serial port {_serialPort.PortName}.");
            _serialPort.DiscardInBuffer();
        }
    }
}
