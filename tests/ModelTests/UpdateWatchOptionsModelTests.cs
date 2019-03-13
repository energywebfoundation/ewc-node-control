using src;
using src.Interfaces;
using src.Models;
using tests.Mocks;
using Xunit;

namespace tests
{
    /// <summary>
    /// Test the Models for correct behaviour
    /// </summary>
    public class UpdateWatchOptionsModelTests
    {
        /// <summary>
        /// Test that the models instantiates with the correct defaults
        /// </summary>
        [Fact]
        public void UpdateWatchOptionsDefaultsTest()
        {
            UpdateWatchOptions sca = new UpdateWatchOptions();
            
            // Interfaces should be null
            Assert.Null(sca.ConfigurationProvider);
            Assert.Null(sca.MessageService);
            Assert.Null(sca.DockerComposeControl);

            // Strings should be empty
            Assert.Equal(string.Empty, sca.ContractAddress);
            Assert.Equal(string.Empty, sca.RpcEndpoint);
            Assert.Equal(string.Empty, sca.ValidatorAddress);
            Assert.Equal(string.Empty, sca.DockerStackPath);
        }
        
        /// <summary>
        /// Test that the model doesn't alter stored data
        /// </summary>
        [Theory]
        [InlineData("0x5f51f49e25b2ba1acc779066a2614eb70a9093a0","http://localhost","0xc3681dfe99730eb45154208cba7b0df7e705f305","/tmp")]
        public void UpdateWatchOptionsSetgetTest(string contractAddr, string rpc, string validatorAddr, string path)
        {
            IDockerComposeControl mdcc = new MockDockerControl();
            IMessageService ms = new MockMessageService();
            IConfigurationProvider cp = new MockConfigProvider();
            
            UpdateWatchOptions sca = new UpdateWatchOptions
            {
                RpcEndpoint = rpc,
                ContractAddress = contractAddr,
                ValidatorAddress = validatorAddr,
                DockerStackPath = path,
                DockerComposeControl = mdcc,
                MessageService = ms,
                ConfigurationProvider = cp 
            };
            
            Assert.Equal(rpc, sca.RpcEndpoint);
            Assert.Equal(contractAddr, sca.ContractAddress);
            Assert.Equal(validatorAddr, sca.ValidatorAddress);
            Assert.Equal(path, sca.DockerStackPath);
            
            Assert.Equal(mdcc, sca.DockerComposeControl);
            Assert.Equal(ms,sca.MessageService);
            Assert.Equal(ms,sca.MessageService);
            Assert.Equal(cp,sca.ConfigurationProvider);
            
        }
    }
}