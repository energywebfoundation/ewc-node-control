using System.Numerics;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using src.Contract;
using Xunit;

namespace tests
{
    public class NetherumDeclarationTests
    {
        [Fact]
        public void ConfirmUpdateFunctionTests()
        {
            typeof(ConfirmUpdateFunction).Should()
                .BeDecoratedWith<FunctionAttribute>(attr => attr.Name == "confirmUpdate")
                .And
                .BeDerivedFrom<FunctionMessage>();
        }
        
        [Fact]
        public void RetrieveUpdateFunctionTests()
        {
            typeof(RetrieveUpdateFunction).Should()
                .BeDecoratedWith<FunctionAttribute>(
                    attr => attr.Name == "RetrieveUpdate" && attr.DTOReturnType == typeof(UpdateStateDto))
                .And
                .BeDerivedFrom<FunctionMessage>()
                .And
                .HaveProperty<string>("ValidatorAddress").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "_targetValidator" &&
                        para.Type == "address" &&
                        para.Order == 1 &&
                        para.Parameter.Indexed);
        }
        
        [Fact]
        public void UpdateStateDtoTests()
        {
            typeof(UpdateStateDto).Should()
                .BeDecoratedWith<FunctionOutputAttribute>()
                .And
                .Implement<IFunctionOutputDTO>()
                .And
                .HaveProperty<ValidatorStateDto>("ValidatorState").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Type == "tuple");
        }

        
        [Fact]
        public void UpdateEventDtoTests()
        {
            typeof(UpdateEventDto).Should()
                .BeDecoratedWith<EventAttribute>(
                    attr => attr.Name == "UpdateAvailable")
                .And
                .HaveProperty<string>("TargetValidator").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "targetValidator" &&
                        para.Type == "address" &&
                        para.Order == 1 &&
                        para.Parameter.Indexed);
            
            typeof(UpdateEventDto).Should()
                .HaveProperty<string>("EventId")
                .Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "eventid" &&
                        para.Type == "uint256" &&
                        para.Order == 2 &&
                        para.Parameter.Indexed);

        }
        
        [Fact]
        public void ValidatorStateDtoTests()
        {
            typeof(ValidatorStateDto).Should()
                .HaveProperty<byte[]>("DockerSha").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "dockerSha" &&
                        para.Type == "bytes" &&
                        para.Order == 1 &&
                        !para.Parameter.Indexed);
            
            typeof(ValidatorStateDto).Should()
                .HaveProperty<string>("DockerName").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "dockerName" &&
                        para.Type == "string" &&
                        para.Order == 2 &&
                        !para.Parameter.Indexed);
            
            typeof(ValidatorStateDto).Should()
                .HaveProperty<byte[]>("ChainSpecSha").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "chainSpecSha" &&
                        para.Type == "bytes" &&
                        para.Order == 3 &&
                        !para.Parameter.Indexed);
            
            typeof(ValidatorStateDto).Should()
                .HaveProperty<string>("ChainSpecUrl").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "chainSpecUrl" &&
                        para.Type == "string" &&
                        para.Order == 4 &&
                        !para.Parameter.Indexed);
            
            typeof(ValidatorStateDto).Should()
                .HaveProperty<bool>("IsSigning").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "isSigning" &&
                        para.Type == "bool" &&
                        para.Order == 5 &&
                        !para.Parameter.Indexed);
            
            typeof(ValidatorStateDto).Should()
                .HaveProperty<BigInteger>("UpdateIntroduced").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "updateIntroduced" &&
                        para.Type == "uint" &&
                        para.Order == 6 &&
                        !para.Parameter.Indexed);
            
            typeof(ValidatorStateDto).Should()
                .HaveProperty<BigInteger>("UpdateConfirmed").Which
                .Should().BeDecoratedWith<ParameterAttribute>(
                    para =>
                        para.Name == "updateConfirmed" &&
                        para.Type == "uint" &&
                        para.Order == 7 &&
                        !para.Parameter.Indexed);

        }
    }
}