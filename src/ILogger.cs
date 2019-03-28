namespace src
{
    /// <summary>
    /// Declares a simple logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a simple message
        /// </summary>
        /// <param name="msg">Message to log</param>
        void Log(string msg);
    }
}