namespace serial_monitor.Models
{
    internal class SerialDataReceivedEventArgs
    {
        public string Data { get; }

        public SerialDataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }
}
