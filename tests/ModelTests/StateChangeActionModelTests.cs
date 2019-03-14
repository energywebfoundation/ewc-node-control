using System;
using System.Diagnostics.CodeAnalysis;
using src;
using src.Models;
using Xunit;

namespace tests
{
    /// <summary>
    /// Test the Models for correct behaviour
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class StateChangeActionModelTests
    {
        /// <summary>
        /// Test that the models instantiates with the correct defaults
        /// </summary>
        [Fact]
        public void StateChangeActionDefaultsTest()
        {
            StateChangeAction sca = new StateChangeAction();
            Assert.Equal(string.Empty, sca.Payload);
            Assert.Equal(string.Empty, sca.PayloadHash);
            Assert.Equal(UpdateMode.Unknown, sca.Mode);
        }
        
        /// <summary>
        /// Test that the model doesn't alter stored data
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="hash"></param>
        /// <param name="mode"></param>
        [Theory]
        [InlineData("parity/parity:v2.3.3","0x12345678912345",UpdateMode.Docker)]
        [InlineData("https://google.com","0x12345678912345",UpdateMode.ChainSpec)]
        public void StateChangeActionSetgetTest(string payload, string hash, UpdateMode mode)
        {
            StateChangeAction sca = new StateChangeAction
            {
                Mode = mode,
                Payload = payload,
                PayloadHash = hash
            };
            
            Assert.Equal(payload, sca.Payload);
            Assert.Equal(hash, sca.PayloadHash);
            Assert.Equal(mode, sca.Mode);
        }
    }
}