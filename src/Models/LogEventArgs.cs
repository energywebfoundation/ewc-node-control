namespace src.Models
{
    /// <summary>
    /// Event that carries the message for a log event
    /// </summary>
    public class LogEventArgs
    {
        /// <summary>
        /// Instantiate event args with a message that should be logged
        /// </summary>
        /// <param name="msg">The message that should be logged</param>
        public LogEventArgs(string msg)
        {
            Message = msg;
        }

        /// <summary>
        /// The Message that gets logged
        /// </summary>
        public string Message { get; }
    }
}