using src.Models;

namespace src.Interfaces
{
    /// <summary>
    /// Describes an interface to persist the current state
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Read the current state from the provider
        /// </summary>
        /// <returns>The current node state</returns>
        NodeState ReadCurrentState();

        /// <summary>
        /// Write/Update the state in persistent storage
        /// </summary>
        /// <param name="newState"></param>
        void WriteNewState(NodeState newState);
    }
}