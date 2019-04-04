using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace src.Contract
{
    /// <summary>
    /// Declares the Nethereum object that handles the RetrieveUpdate function of the smart contract
    /// </summary>
    [Function("RetrieveUpdate",typeof(UpdateStateDto))]
    public class RetrieveUpdateFunction : FunctionMessage
    {
        /// <summary>
        /// Function parameter targetValidator that is supplied to the contract
        /// </summary>
        [Parameter("address", "_targetValidator", 1, true)]
        public string ValidatorAddress { get; set; }
    }
    
    
    /// <summary>
    /// Get function to retrieve the current node control contract address
    /// </summary>
    [Function("nodeControlContract", "address")]
    public class NodeControlContractFunction : FunctionMessage {}

}