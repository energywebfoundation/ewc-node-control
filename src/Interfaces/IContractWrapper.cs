using System.Threading.Tasks;
using src.Models;

namespace src.Interfaces
{
    /// <summary>
    /// Describes interaction with the node control smart contract
    /// </summary>
    public interface IContractWrapper
    {
        /// <summary>
        /// Asynchronously check the contract for a new update event
        /// </summary>
        /// <returns></returns>
        Task<bool> HasNewUpdate();
        
        /// <summary>
        /// Asynchronously get the currently expected state from the smart contract
        /// </summary>
        /// <returns></returns>
        Task<NodeState> GetExpectedState();
        
        /// <summary>
        /// Asynchronously send the update confirmation transaction to the smart contract
        /// </summary>
        /// <returns></returns>
        Task ConfirmUpdate();
    }
}