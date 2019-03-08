namespace src.Models
{
    public class LogArgs
    {
        public LogArgs(string msg)
        {
            Message = msg;
        }

        public string Message { get; }
    }
}