using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace src.Contract
{
    public class ContractWrapper : IContractWrapper
    {
        private readonly ContractHandler _contractHandler;
        private readonly string _validatorAddress;

        public ContractWrapper(string contractAddress, string rpcEndpoint, string validatorAddress)
        {
            Web3 web3 = new Web3(rpcEndpoint);
            
            _contractHandler = web3.Eth.GetContractHandler(contractAddress);
            _validatorAddress = validatorAddress;
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
}