using System;
using src.Interfaces;
using src.Models;

namespace src
{
    /// <inheritdoc />
    /// <summary>
    /// Writes messages just to STDOUT
    /// </summary>
    public class ConsoleMessageService : IMessageService
    {
        /// <inheritdoc />
        /// <summary>
        /// Write error message to console
        /// </summary>
        public void SendErrorMessage(string subject, string errorMessage, NodeState state)
        {
            Console.WriteLine($"[MSG | {subject}] {errorMessage}" );
        }
    }
}