using src.Models;

namespace src.Interfaces
{
    /// <summary>
    /// Describes a way to send messages to network operation humans
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Send a message 
        /// </summary>
        /// <param name="subject">The subject of the message</param>
        /// <param name="errorMessage">A detailed error message</param>
        /// <param name="state">The state that caused the issue</param>
        void SendErrorMessage(string subject, string errorMessage, NodeState state);
    }
}