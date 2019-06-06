using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace src.Contract
{
    /// <summary>
    /// The node state representation DTO from the smart contract
    /// </summary>
    public class ValidatorStateDto
    {
        /// <summary>
        /// SHA256 Checksum of the docker image
        /// </summary>
        /// <remarks>Equal to the docker image id</remarks>
        [Parameter("bytes","dockerSha")]
        public byte[] DockerSha { get; set; }

        /// <summary>
        /// The docker image of this state
        /// </summary>
        /// <example>parity/parity:v2.3.3</example>
        [Parameter("string","dockerName",2)]
        public string DockerName { get; set; }

        /// <summary>
        /// SHA256 Hash of the chainspec file contents as hex string
        /// </summary>
        [Parameter("bytes","chainSpecSha",3)]
        public byte[] ChainSpecSha { get; set; }

        /// <summary>
        /// HTTPS URL of the chain spec file
        /// </summary>
        [Parameter("string","chainSpecUrl",4)]
        public string ChainSpecUrl { get; set; }

        /// <summary>
        /// Flag that determines if the validator is actively signing blocks or not.
        /// </summary>
        [Parameter("bool","isSigning",5)]
        public bool IsSigning { get; set; }

        /// <summary>
        /// Timestamp of the block when the update was introduced
        /// </summary>
        [Parameter("uint","updateIntroduced",6)]
        public BigInteger UpdateIntroduced { get; set; }

        /// <summary>
        /// Timestamp of the block when the update was confirmed using ConfirmUpdate
        /// </summary>
        [Parameter("uint","updateConfirmed",7)]
        public BigInteger UpdateConfirmed { get; set; }

    }
}