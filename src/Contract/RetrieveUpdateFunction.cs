using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace src.Contract
{
    [Function("RetrieveUpdate",typeof(UpdateStateDto))]
    public class RetrieveUpdateFunction : FunctionMessage
    {
        [Parameter("address", "_targetValidator", 1, true)]
        public string ValidatorAddress { get; set; }
    }
}