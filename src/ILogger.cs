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

        /// <summary>
        /// Logs an error 
        /// </summary>
        /// <param name="subject">Module that thrown the error</param>
        /// <param name="errorMessage">Error message</param>
        void Error(string subject, string errorMessage);
    }
}