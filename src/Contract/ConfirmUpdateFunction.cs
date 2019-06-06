using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;

namespace src.Contract
{
    /// <summary>
    /// The nethereum function definition of the ConfirmUpdate contract function
    /// </summary>
    [Function("confirmUpdate")]
    public class ConfirmUpdateFunction : FunctionMessage {}
}