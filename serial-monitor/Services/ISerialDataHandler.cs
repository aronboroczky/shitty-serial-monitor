using serial_monitor.Models;

namespace serial_monitor.Services
{
    internal interface ISerialDataHandler
    {
        event EventHandler<SerialDataReceivedEventArgs> SerialDataReceived;
        event EventHandler<SerialConnectionEventArgs> SerialConnectionChanged;

        public void Open();
        public void Close();
        public string GetPort();
        public bool IsConnected();
        public void ClearBuffer();
    }
}
