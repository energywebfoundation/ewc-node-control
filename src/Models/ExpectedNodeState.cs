using System.Numerics;

namespace src
{
    public class ExpectedNodeState
    {
        public string DockerImage { get; set; }
        public string DockerChecksum { get; set; }
        public string ChainspecUrl { get; set; }
        public string ChainspecChecksum { get; set; }
        public bool IsSigning { get; set; }
        public BigInteger UpdateIntroducedBlock { get; set; }
    }
}