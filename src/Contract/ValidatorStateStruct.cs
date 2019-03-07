using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace src.Contract
{
    public class ValidatorStateStruct
    {
        [Parameter("bytes","dockerSha")]
        public byte[] DockerSha { get; set; }
        
        [Parameter("string","dockerName",2)]
        public string DockerName { get; set; }
        
        [Parameter("bytes","chainSpecSha",3)]
        public byte[] ChainSpecSha { get; set; }
        
        [Parameter("string","chainSpecUrl",4)]
        public string ChainSpecUrl { get; set; }
        
        [Parameter("bool","isSigning",5)]
        public bool IsSigning { get; set; }
        
        [Parameter("uint","updateIntroduced",6)]
        public BigInteger Updateintroduced { get; set; }
        
        [Parameter("uint","updateConfirmed",7)]
        public BigInteger UpdateConfirmed { get; set; }
        
    }
}