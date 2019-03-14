using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using src.Models;
using Xunit;

namespace tests
{


    public class UpdateModeEnumTests
    {
        /// <summary>
        /// Verifies that the enum values didn't change
        /// </summary>
        [Fact]
        public void ShouldHaveCorrectValues()
        {
            UpdateMode.Unknown.Should().Be(0);
            UpdateMode.Docker.Should().Be(1);
            UpdateMode.ChainSpec.Should().Be(2);
            UpdateMode.ToggleSigning.Should().Be(3);
        }
    }
    
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
        public void ShouldGetSetProperly(string dImg, string dChksum, string cs, string csSum, bool isSigning, int block)
        {
            NodeState ens = new NodeState
            {
                IsSigning = isSigning,
                DockerImage = dImg,
                ChainspecUrl = cs,
                DockerChecksum = dChksum,
                ChainspecChecksum = csSum,
                UpdateIntroducedBlock = block
            };
            
            Assert.Equal(isSigning, ens.IsSigning);
            Assert.Equal(dImg, ens.DockerImage);
            Assert.Equal(dChksum, ens.DockerChecksum);
            Assert.Equal(cs, ens.ChainspecUrl);
            Assert.Equal(csSum, ens.ChainspecChecksum);
            Assert.Equal(block, ens.UpdateIntroducedBlock);
            
        }
    }
}