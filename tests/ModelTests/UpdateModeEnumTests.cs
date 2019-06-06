using FluentAssertions;
using src.Models;
using Xunit;

namespace tests.ModelTests
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
}