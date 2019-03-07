using System;
using src;
using src.Contract;
using Xunit;

namespace tests
{
    public class ContractTests
    {
        [Fact]
        public void QueryContract()
        {

            string contractAddress = "0x5f51f49e25b2ba1acc779066a2614eb70a9093a0";
            string rpc = "http://localhost:8545";
            string validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";

            ContractWrapper cw = new ContractWrapper(contractAddress,rpc,validatorAddress);
            var state = cw.GetExpectedState().Result;
            Assert.Equal("parity/parity:v2.3.3",state.DockerImage);
        }
        
        [Fact]
        public void ConfirmUpdate()
        {

            string contractAddress = "0x5f51f49e25b2ba1acc779066a2614eb70a9093a0";
            string rpc = "http://localhost:8545";
            string validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";
            
            ContractWrapper cw = new ContractWrapper(contractAddress,rpc,validatorAddress);
            cw.ConfirmUpdate().Wait();
            
        }
    }
}
