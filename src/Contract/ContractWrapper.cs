using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using src.Interfaces;
using src.Models;

namespace src.Contract
{
    public class ContractWrapper : IContractWrapper
    {
        private readonly ContractHandler _contractHandler;
        private readonly string _validatorAddress;
        private readonly Event<UpdateEventDTO> _updateEventHandler;
        private HexBigInteger _lastBlock;
        private Web3 _web3;

        public ContractWrapper(string contractAddress, string rpcEndpoint, string validatorAddress)
        {
            _web3 = new Web3(rpcEndpoint);
            
            _contractHandler = _web3.Eth.GetContractHandler(contractAddress);
            _validatorAddress = validatorAddress;
            _updateEventHandler = _web3.Eth.GetEvent<UpdateEventDTO>(contractAddress);

        }

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

        public async Task<ExpectedNodeState> GetExpectedState()
        {

            UpdateStateDto contractResponse =  await _contractHandler.QueryDeserializingToObjectAsync<RetrieveUpdateFunction, UpdateStateDto>(new RetrieveUpdateFunction
            {
                ValidatorAddress = _validatorAddress,
                FromAddress = _validatorAddress
            });
            return new ExpectedNodeState
            {
                DockerImage = contractResponse.ValidatorState.DockerName,
                DockerChecksum = ConvertBytesToHexString(contractResponse.ValidatorState.DockerSha),
                IsSigning = contractResponse.ValidatorState.IsSigning,
                ChainspecUrl = contractResponse.ValidatorState.ChainSpecUrl,
                ChainspecChecksum = ConvertBytesToHexString(contractResponse.ValidatorState.ChainSpecSha),
                UpdateIntroducedBlock = contractResponse.ValidatorState.Updateintroduced
            };
        }

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

    [Event("UpdateAvailable")]
    public class UpdateEventDTO
    {
        [Parameter("address","targetValidator",1,true)]
        public string TargetValidator { get; set; }
        
        [Parameter("uint256","eventid",2,true)]
        public string EventId { get; set; }
    }
}