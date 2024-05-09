using System.IO.Ports;

namespace serial_monitor.Settings
{
    internal class SerialPortSettings
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public Parity Parity { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }

    }
}
