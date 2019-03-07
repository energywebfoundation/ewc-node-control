using Nethereum.ABI.FunctionEncoding.Attributes;

namespace src.Contract
{
    [FunctionOutput]
    public class UpdateStateDto : IFunctionOutputDTO
    {
        [Parameter("tuple")]
        public ValidatorStateStruct ValidatorState { get; set; }
    }
}