namespace serial_monitor.Models
{
    internal class SerialPortOptions
    {
        public string PortName { get; set; } = "COM1";
        public int BaudRate { get; set; } = 115200;

    }
}
