using System;

namespace src
{
    /// <inheritdoc />
    /// <summary>
    /// Implements a simple console logger
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc />
        /// <summary>
        /// Logs the msg to the console
        /// </summary>
        /// <param name="msg">Message to print to console</param>
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}