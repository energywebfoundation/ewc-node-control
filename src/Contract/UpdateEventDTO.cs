using Nethereum.ABI.FunctionEncoding.Attributes;

namespace src.Contract
{
    /// <summary>
    /// Declares the on-chain event UpdateAvailable
    /// </summary>
    [Event("UpdateAvailable")]
    public class UpdateEventDto
    {
        /// <summary>
        /// Address of the validator that the update is targeted for
        /// </summary>
        [Parameter("address","targetValidator",1,true)]
        public string TargetValidator { get; set; }
        
        /// <summary>
        /// Id of the event
        /// </summary>
        [Parameter("uint256","eventid",2,true)]
        public string EventId { get; set; }
    }
}