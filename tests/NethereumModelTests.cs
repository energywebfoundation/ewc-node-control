using System.Numerics;
using FluentAssertions;
using src.Contract;
using Xunit;

namespace tests
{
    public class NethereumModelTests
    {
        [Fact]
        public void RetrieveUpdateFunctionModelTests()
        {
            RetrieveUpdateFunction ruf = new RetrieveUpdateFunction
            {
                ValidatorAddress = "0x00000"
            };

            ruf.ValidatorAddress.Should().Be("0x00000");

        }
        
        [Fact]
        public void UpdateStateDtoModelTests()
        {
            var state = new ValidatorStateDto();
            
            UpdateStateDto ruf = new UpdateStateDto
            {
                ValidatorState = state
            };

            ruf.ValidatorState.Should().Be(state);

        }
        
        [Fact]
        public void UpdateEventDtoModelTests()
        {
            UpdateEventDto ueDto = new UpdateEventDto
            {
                TargetValidator = "0x00000",
                EventId = "1234"
            };

            ueDto.TargetValidator.Should().Be("0x00000");
            ueDto.EventId.Should().Be("1234");

        }
        
        [Fact]
        public void ValidatorStateDtoModelTests()
        {
            ValidatorStateDto vsDto = new ValidatorStateDto
            {
                IsSigning = true,
                DockerSha = new byte[]{0x0,0x1,0x2},
                ChainSpecSha = new byte[]{0x4,0x5,0x6},
                ChainSpecUrl = "http://foo.bar",
                DockerName = "parity/parity:v2.3.4",
                UpdateConfirmed = BigInteger.One,
                UpdateIntroduced = BigInteger.MinusOne
            };

            vsDto.IsSigning.Should().Be(true);
            vsDto.DockerSha.Should().ContainInOrder(new byte[] {0x0, 0x1, 0x2});
            vsDto.ChainSpecSha.Should().ContainInOrder(new byte[] {0x4, 0x5, 0x6});
            vsDto.ChainSpecUrl.Should().Be("http://foo.bar");
            vsDto.DockerName.Should().Be("parity/parity:v2.3.4");
            vsDto.UpdateConfirmed.Should().Be(BigInteger.One);
            vsDto.UpdateIntroduced.Should().Be(BigInteger.MinusOne);

        }
    }
}