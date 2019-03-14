using Nethereum.ABI.FunctionEncoding.Attributes;

namespace src.Contract
{
    /// <summary>
    /// Declares the return of the RetrieveUpdate contract function
    /// </summary>
    [FunctionOutput]
    public class UpdateStateDto : IFunctionOutputDTO
    {
        /// <summary>
        /// Map the returning tuple to the ValidatorState struct tuple
        /// </summary>
        [Parameter("tuple")]
        public ValidatorStateDTO ValidatorState { get; set; }
    }
}