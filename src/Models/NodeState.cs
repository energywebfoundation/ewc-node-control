using System.Numerics;

namespace src.Models
{
    /// <summary>
    /// Represents the state of a node
    /// </summary>
    public class NodeState
    {
        /// <summary>
        /// The docker image of this state
        /// </summary>
        /// <example>parity/parity:v2.3.3</example>
        public string DockerImage { get; set; } = string.Empty;

        /// <summary>
        /// SHA256 Checksum of the docker image
        /// </summary>
        /// <remarks>Equal to the docker image id</remarks>
        public string DockerChecksum { get; set; } = string.Empty;

        /// <summary>
        /// HTTPS URL of the chain spec file
        /// </summary>
        public string ChainspecUrl { get; set; } = string.Empty;
        /// <summary>
        /// SHA256 Hash of the chainspec file contents as hex string
        /// </summary>
        public string ChainspecChecksum { get; set; } = string.Empty;
        /// <summary>
        /// Flag that determines if the validator is actively signing blocks or not.
        /// </summary>
        public bool IsSigning { get; set; }

        /// <summary>
        /// Timestamp of the block when the update was introduced
        /// </summary>
        public BigInteger UpdateIntroducedBlock { get; set; } = 0;
    }
}