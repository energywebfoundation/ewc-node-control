using System;
using System.Collections;
using FluentAssertions;
using src;
using src.Models;
using Xunit;

namespace tests
{
    public class ConfigBuilderTests
    {
        [Fact]
        public void ShouldBuildFromEnvironment()
        {
            Hashtable envHt = new Hashtable
            {
                {"CONTRACT_ADDRESS","0x12345"},
                {"STACK_PATH","/foo/path"},
                {"RPC_ENDPOINT","http://my.rpc.endpoint"},
                {"VALIDATOR_ADDRESS","0xabfed12345"},
            };

            UpdateWatchOptions watchOpts = ConfigBuilder.BuildConfigurationFromEnvironment(envHt);
            
            // Verify correct env reading
            watchOpts.RpcEndpoint.Should().Be("http://my.rpc.endpoint");
            watchOpts.ValidatorAddress.Should().Be("0xabfed12345");
            watchOpts.ContractAddress.Should().Be("0x12345");
            watchOpts.DockerStackPath.Should().Be("/foo/path");
        }
        
        [Fact]
        public void ShouldBuildWithDefaults()
        {
            Hashtable envHt = new Hashtable();
            
            UpdateWatchOptions watchOpts = ConfigBuilder.BuildConfigurationFromEnvironment(envHt);
            
            // Verify correct env reading
            watchOpts.RpcEndpoint.Should().Be("http://localhost:8545");
            watchOpts.ValidatorAddress.Should().Be(string.Empty);
            watchOpts.ContractAddress.Should().Be(string.Empty);
            watchOpts.DockerStackPath.Should().Be("./demo-stack");
        }

        [Fact]
        public void ShouldThrowOnNullEnv()
        {
            Assert.Throws<ArgumentNullException>(() => { _ = ConfigBuilder.BuildConfigurationFromEnvironment(null); });
        }
    }
}