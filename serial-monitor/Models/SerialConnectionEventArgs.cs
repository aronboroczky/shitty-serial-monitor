using serial_monitor.Models.Enums;

namespace serial_monitor.Models
{
    internal class SerialConnectionEventArgs
    {
        public string PortName { get; set; }
        public SerialConnectionState ConnectionState { get; set; }

        public SerialConnectionEventArgs(string portName, SerialConnectionState connectionState)
        {
            PortName = portName;
            ConnectionState = connectionState;
        }
    }
}
