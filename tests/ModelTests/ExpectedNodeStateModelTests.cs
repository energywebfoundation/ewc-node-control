using System.Diagnostics.CodeAnalysis;
using src.Models;
using Xunit;

namespace tests.ModelTests
{
    /// <summary>
    /// Test the Models for correct behaviour
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExpectedNodeStateModelTests
    {
        /// <summary>
        /// Test that the models instantiates with the correct defaults
        /// </summary>
        [Fact]
        public void ShouldHaveCorrectDefaults()
        {
            NodeState ens = new NodeState();
            Assert.Equal(string.Empty, ens.DockerImage);
            Assert.Equal(string.Empty, ens.DockerChecksum);
            Assert.Equal(string.Empty, ens.ChainspecUrl);
            Assert.Equal(string.Empty, ens.ChainspecChecksum);
            Assert.Equal(0, ens.UpdateIntroducedBlock);
            Assert.False(ens.IsSigning);
        }

        /// <summary>
        /// Test that the model doesn't alter stored data
        /// </summary>
        [Theory]
        [InlineData("parity/parity:v2.3.3","0x12345678912345","https://google.com","0x12345678912345",true,256)]
        public void ShouldGetSetProperly(string dockerImage, string dockerChecksum, string chainspec, string chainspecChecksum, bool isSigning, int block)
        {
            NodeState ens = new NodeState
            {
                IsSigning = isSigning,
                DockerImage = dockerImage,
                ChainspecUrl = chainspec,
                DockerChecksum = dockerChecksum,
                ChainspecChecksum = chainspecChecksum,
                UpdateIntroducedBlock = block
            };

            Assert.Equal(isSigning, ens.IsSigning);
            Assert.Equal(dockerImage, ens.DockerImage);
            Assert.Equal(dockerChecksum, ens.DockerChecksum);
            Assert.Equal(chainspec, ens.ChainspecUrl);
            Assert.Equal(chainspecChecksum, ens.ChainspecChecksum);
            Assert.Equal(block, ens.UpdateIntroducedBlock);

        }
    }
}