using System;
using System.Numerics;

namespace src.Models
{
    public class ExpectedNodeState
    {
        public string DockerImage { get; set; } = String.Empty;
        public string DockerChecksum { get; set; } = String.Empty;
        public string ChainspecUrl { get; set; } = String.Empty;
        public string ChainspecChecksum { get; set; } = String.Empty;
        public bool IsSigning { get; set; }
        public BigInteger UpdateIntroducedBlock { get; set; } = 0;
    }
}