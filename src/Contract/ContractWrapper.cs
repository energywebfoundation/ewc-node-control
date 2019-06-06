using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
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
        private ContractHandler _contractHandler;

        /// <summary>
        ///  The ethereum address of the controlled validator
        /// </summary>
        private readonly string _validatorAddress;

        /// <summary>
        /// Handler for the on-chain UpdateEvent
        /// </summary>
        private Event<UpdateEventDto> _updateEventHandler;

        /// <summary>
        /// Last checked block
        /// </summary>
        private HexBigInteger _lastBlock;

        /// <summary>
        /// Instantiated web3 object
        /// </summary>
        private readonly Web3 _web3;

        private readonly ILogger _logger;
        private string _ncContractAddress;
        private readonly string _blockNumberFile;
        private readonly string _lookupContractAddress;

        /// <summary>
        /// Instantiates the a new wrapper
        /// </summary>
        /// <param name="lookupContractAddress">Address of the address lookup smart contract</param>
        /// <param name="rpcEndpoint">HTTP URL to the JSON-RPC endpoint</param>
        /// <param name="validatorAddress">The ethereum address of the controlled validator</param>
        /// <param name="logger">Logger implementation to use</param>
        /// <param name="keyPw">Password to decrypt the key file</param>
        /// <param name="keyJson">The validator key as encrypted JSON</param>
        /// <param name="blockNumberFile">File to read and write the last checked block number</param>
        public ContractWrapper(string lookupContractAddress, string rpcEndpoint, string validatorAddress, ILogger logger,string keyPw,string keyJson, string blockNumberFile)
        {
            _validatorAddress = validatorAddress;
            _lookupContractAddress = lookupContractAddress;
            _blockNumberFile = blockNumberFile;

            _logger = logger;

            // load the validator account
            //ManagedAccount acc = new ManagedAccount(validatorAddress,keyPw);
            Account acc = Account.LoadFromKeyStore(keyJson, keyPw);

            // create a web 3 instance

            _web3 = new Web3(acc, rpcEndpoint);

            GetContract();


            // get block
            GetLastCheckedBlock();

            Log($"Starting to listen from block #{_lastBlock.Value}");

        }

        private void GetContract()
        {
            // hook up to the contract and event
            ContractHandler lookupContractHandler = _web3.Eth.GetContractHandler(_lookupContractAddress);
            string ncContractAddress = lookupContractHandler
                .QueryAsync<NodeControlContractFunction, string>().Result;


            if (string.IsNullOrWhiteSpace(ncContractAddress))
            {
                Log($"Unable to retrieve contract address from lookup at {_lookupContractAddress}");
                throw new Exception("Unable to retrieve node control contract address from lookup.");
            }

            Log($"Retrieved Contract Address {ncContractAddress} from lookup at {_lookupContractAddress}");

            _contractHandler = _web3.Eth.GetContractHandler(ncContractAddress);
            _updateEventHandler = _web3.Eth.GetEvent<UpdateEventDto>(ncContractAddress);
            _ncContractAddress = ncContractAddress;
        }

        private void GetLastCheckedBlock()
        {
            _lastBlock = null;

            try
            {
                if (File.Exists(_blockNumberFile))
                {
                    string hexValueFromFile = File.ReadAllText(_blockNumberFile);
                    if (!string.IsNullOrWhiteSpace(hexValueFromFile))
                    {
                        _lastBlock = new HexBigInteger(hexValueFromFile);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("ContractWrapper","Unable to read last block from file: " + e.Message);
            }

            if (_lastBlock == null)
            {
                // Unable to read block from file
                _lastBlock = _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync().Result;
            }
        }

        private void PersistLastCheckedBlock()
        {
            string blockNumberAsHex = _lastBlock.HexValue;

            try
            {
                if (File.Exists(_blockNumberFile))
                {
                    File.WriteAllText(_blockNumberFile,blockNumberAsHex);
                }
            }
            catch (Exception e)
            {
                _logger.Error("ContractWrapper","Unable to persist last checked block to file: " + e.Message);
            }
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
            HexBigInteger curBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

            // Make sure correct contract is used
            GetContract();

            // check block range for new events
            NewFilterInput filterInput = _updateEventHandler.CreateFilterInput(new BlockParameter(_lastBlock),new BlockParameter(curBlock));
            List<EventLog<UpdateEventDto>> outstandingEvents = await  _updateEventHandler.GetAllChanges(filterInput);

            Log($"Found {outstandingEvents.Count} update events. Checking if we got addressed.");
            // save current block number
            _lastBlock = curBlock;
            PersistLastCheckedBlock();
            return outstandingEvents.Any(x => x.Event.TargetValidator == _validatorAddress);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<NodeState> GetExpectedState()
        {
            // Make sure the correct contract is referenced
            GetContract();

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

            IContractTransactionHandler<ConfirmUpdateFunction> updateTxHandler = _web3.Eth.GetContractTransactionHandler<ConfirmUpdateFunction>();
            ConfirmUpdateFunction updateTx = new ConfirmUpdateFunction
            {
                FromAddress = _validatorAddress,
                Gas = new BigInteger(500000)
            };

            TransactionReceipt confirmResponse = await updateTxHandler.SendRequestAndWaitForReceiptAsync(_ncContractAddress, updateTx);


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