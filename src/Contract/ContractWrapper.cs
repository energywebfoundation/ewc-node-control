using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using src.Interfaces;
using src.Models;

namespace src.Contract
{
    /// <summary>
    /// Nethereum-based implementation of the IContractWrapper interface
    /// </summary>
    public class ContractWrapper : IContractWrapper
    {
        /// <summary>
        /// Instantiated contract handler
        /// </summary>
        private readonly ContractHandler _contractHandler;
        
        /// <summary>
        ///  The ethereum address of the controlled validator
        /// </summary>
        private readonly string _validatorAddress;
        
        /// <summary>
        /// Handler for the on-chain UpdateEvent
        /// </summary>
        private readonly Event<UpdateEventDTO> _updateEventHandler;
        
        /// <summary>
        /// Last checked block
        /// </summary>
        private HexBigInteger _lastBlock;
        
        /// <summary>
        /// Instantiated web3 object
        /// </summary>
        private readonly Web3 _web3;

        /// <summary>
        /// Instantiates the a new wrapper
        /// </summary>
        /// <param name="contractAddress">Address of the smart contract</param>
        /// <param name="rpcEndpoint">HTTP URL to the JSON-RPC endpoint</param>
        /// <param name="validatorAddress">The ethereum address of the controlled validator</param>
        public ContractWrapper(string contractAddress, string rpcEndpoint, string validatorAddress)
        {
            _validatorAddress = validatorAddress;
            
            // create a web 3 instance
            _web3 = new Web3(rpcEndpoint);
            
            // hook up to the contract and event
            _contractHandler = _web3.Eth.GetContractHandler(contractAddress);
            _updateEventHandler = _web3.Eth.GetEvent<UpdateEventDTO>(contractAddress);

        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasNewUpdate()
        {
            // get current block number
            var curBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            // check block range for new events
            NewFilterInput filterInput = _updateEventHandler.CreateFilterInput(_lastBlock, curBlock);
            List<EventLog<UpdateEventDTO>> outrstandingEvents = await  _updateEventHandler.GetAllChanges(filterInput);

            // save current block number
            _lastBlock = curBlock;
            return outrstandingEvents.Any(x => x.Event.TargetValidator == _validatorAddress);
        } 

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<NodeState> GetExpectedState()
        {

            UpdateStateDto contractResponse =  await _contractHandler.QueryDeserializingToObjectAsync<RetrieveUpdateFunction, UpdateStateDto>(new RetrieveUpdateFunction
            {
                ValidatorAddress = _validatorAddress,
                FromAddress = _validatorAddress
            });
            return new NodeState
            {
                DockerImage = contractResponse.ValidatorState.DockerName,
                DockerChecksum = ConvertBytesToHexString(contractResponse.ValidatorState.DockerSha),
                IsSigning = contractResponse.ValidatorState.IsSigning,
                ChainspecUrl = contractResponse.ValidatorState.ChainSpecUrl,
                ChainspecChecksum = ConvertBytesToHexString(contractResponse.ValidatorState.ChainSpecSha),
                UpdateIntroducedBlock = contractResponse.ValidatorState.Updateintroduced
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="T:src.Contract.ContractException">Thrown if the transaction can not be completed</exception>
        public async Task ConfirmUpdate()
        {

            TransactionReceipt confirmResponse = await _contractHandler.SendRequestAndWaitForReceiptAsync(new ConfirmUpdateFunction
            {
                FromAddress = _validatorAddress
            });
            bool? hasErrors = confirmResponse.HasErrors();
            if (hasErrors.HasValue && hasErrors.Value)
            {
                throw new ContractException("Unable to confirm update");
            }
            
        }
        
        /// <summary>
        /// Converts a byte array to a hex string
        /// </summary>
        /// <param name="bytes">byte array to convert</param>
        /// <returns>the hex string representation of the byte array</returns>
        private static string ConvertBytesToHexString(IEnumerable<byte> bytes) {
            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();  
            foreach (byte t in bytes)
            {
                builder.Append(t.ToString("x2"));
            }  
            return builder.ToString();
        }
    }
}