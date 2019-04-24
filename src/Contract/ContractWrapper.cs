using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts.Managed;
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
        private readonly Event<UpdateEventDto> _updateEventHandler;
        
        /// <summary>
        /// Last checked block
        /// </summary>
        private HexBigInteger _lastBlock;
        
        /// <summary>
        /// Instantiated web3 object
        /// </summary>
        private readonly Web3 _web3;

        private ILogger _logger;
        private string _ncContractAddress;

        /// <summary>
        /// Instantiates the a new wrapper
        /// </summary>
        /// <param name="lookupContractAddress">Address of the address lookup smart contract</param>
        /// <param name="rpcEndpoint">HTTP URL to the JSON-RPC endpoint</param>
        /// <param name="validatorAddress">The ethereum address of the controlled validator</param>
        public ContractWrapper(string lookupContractAddress, string rpcEndpoint, string validatorAddress, ILogger logger,string keyPw)
        {
            _validatorAddress = validatorAddress;
            _logger = logger;
            
            // load the validator account
            ManagedAccount acc = new ManagedAccount(validatorAddress,keyPw);
            
            // create a web 3 instance
            
            _web3 = new Web3(acc, rpcEndpoint);
            
            // hook up to the contract and event
            ContractHandler lookupContractHandler = _web3.Eth.GetContractHandler(lookupContractAddress);
            string ncContractAddress = lookupContractHandler
                .QueryAsync<NodeControlContractFunction, string>(null, null).Result;


            if (string.IsNullOrWhiteSpace(ncContractAddress))
            {
                    Log($"Unable to retrieve contract address from lookup at {lookupContractAddress}");
                    throw new Exception("Unable to retrieve node control contract address from lookup.");
            }

            Log($"Retrieved Contract Address {ncContractAddress} from lookup at {lookupContractAddress}");

            _contractHandler = _web3.Eth.GetContractHandler(ncContractAddress);
            _updateEventHandler = _web3.Eth.GetEvent<UpdateEventDto>(ncContractAddress);
            _ncContractAddress = ncContractAddress;
            _lastBlock = _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result;
            Log($"Starting to listen from block #{_lastBlock.Value}");

        }

        private void Log(string msg)
        {
            _logger.Log($"[CONTRACT-WRAPPER] {msg}");
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
            NewFilterInput filterInput = _updateEventHandler.CreateFilterInput(new BlockParameter(_lastBlock),new BlockParameter(curBlock));
            List<EventLog<UpdateEventDto>> outrstandingEvents = await  _updateEventHandler.GetAllChanges(filterInput);

            Log($"Found {outrstandingEvents.Count} update events. Checking if we got addressed.");
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
                UpdateIntroducedBlock = contractResponse.ValidatorState.UpdateIntroduced
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        /// <exception cref="T:src.Contract.ContractException">Thrown if the transaction can not be completed</exception>
        public async Task ConfirmUpdate()
        {

            var updateTxHandler = _web3.Eth.GetContractTransactionHandler<ConfirmUpdateFunction>();
            var updateTx = new ConfirmUpdateFunction
            {
                FromAddress = _validatorAddress,
                Gas = new BigInteger(500000)
            };

            var confirmResponse = await updateTxHandler.SendRequestAndWaitForReceiptAsync(_ncContractAddress, updateTx);
            
            
            //TransactionReceipt confirmResponse = await _contractHandler.SendRequestAndWaitForReceiptAsync(new ConfirmUpdateFunction
            //{
            //    FromAddress = _validatorAddress,
            //    Gas = new BigInteger(500000)
            //});
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