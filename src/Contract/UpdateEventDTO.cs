using Nethereum.ABI.FunctionEncoding.Attributes;

namespace src.Contract
{
    /// <summary>
    /// Declares the on-chain event UpdateAvailable
    /// </summary>
    [Event("UpdateAvailable")]
    public class UpdateEventDto: IEventDTO
    {
        /// <summary>
        /// Address of the validator that the update is targeted for
        /// </summary>
        [Parameter("address","targetValidator",1,true)]
        public string TargetValidator { get; set; }

    }
}