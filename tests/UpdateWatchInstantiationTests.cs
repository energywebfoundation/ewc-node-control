using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using src;
using src.Interfaces;
using src.Models;
using tests.Mocks;
using Xunit;

namespace tests
{
    public class UpdateWatchInstantiationTests
    {

        public static IEnumerable<object[]> GenerateMissingDependecyTestcases()
        {

            // Missing docker provider
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ContractAddress = "0x0",
                    ValidatorAddress = "0x0",
                    DockerStackPath = "/some/path",

                    MessageService = new MockMessageService(),
                    ConfigurationProvider = new MockConfigProvider(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't carry a docker compose control implementation"
            };

            // Missing configuration provider
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ContractAddress = "0x0",
                    ValidatorAddress = "0x0",
                    DockerStackPath = "/some/path",

                    MessageService = new MockMessageService(),
                    DockerComposeControl = new MockDockerControl(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't carry a configuration provider implementation"
            };

            // Missing message service
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ContractAddress = "0x0",
                    ValidatorAddress = "0x0",
                    DockerStackPath = "/some/path",
                    DockerComposeControl = new MockDockerControl(),
                    ConfigurationProvider = new MockConfigProvider(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't carry a message service implementation"
            };

            // Missing contract wrapper service
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ContractAddress = "0x0",
                    ValidatorAddress = "0x0",
                    DockerStackPath = "/some/path",
                    MessageService = new MockMessageService(),
                    DockerComposeControl = new MockDockerControl(),
                    ConfigurationProvider = new MockConfigProvider()
                },
                "Options didn't carry a ContractWrapper implementation"
            };
            
            // Missing rpc endpoint 
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    ContractAddress = "0x0",
                    ValidatorAddress = "0x0",
                    DockerStackPath = "/some/path",
                    MessageService = new MockMessageService(),
                    DockerComposeControl = new MockDockerControl(),
                    ConfigurationProvider = new MockConfigProvider(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't provide an rpc url"
            };

            // Missing contract address
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ValidatorAddress = "0x0",
                    DockerStackPath = "/some/path",
                    MessageService = new MockMessageService(),
                    DockerComposeControl = new MockDockerControl(),
                    ConfigurationProvider = new MockConfigProvider(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't provide a contract address"
            };

            // missing validator address
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ContractAddress = "0x0",
                    DockerStackPath = "/some/path",
                    MessageService = new MockMessageService(),
                    DockerComposeControl = new MockDockerControl(),
                    ConfigurationProvider = new MockConfigProvider(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't provide a validator address"
            };

            // Missing docker stack path
            yield return new object[]
            {
                new UpdateWatchOptions
                {
                    RpcEndpoint = "http://example.com",
                    ContractAddress = "0x0",
                    ValidatorAddress = "0x0",
                    MessageService = new MockMessageService(),
                    DockerComposeControl = new MockDockerControl(),
                    ConfigurationProvider = new MockConfigProvider(),
                    ContractWrapper = new MockContractWrapper()
                },
                "Options didn't provide a docker stack path"
            };

        }

        [Theory]
        [MemberData(nameof(GenerateMissingDependecyTestcases))]
        public void ShouldThrowOnMissingOptions(UpdateWatchOptions badOpts, string expectedMessage)
        {

            Action ctor = () => { _ = new UpdateWatch(badOpts); };
            ctor.Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Theory]
        [InlineData("foobar", "c3ab8ff13720e8ad9047dd39466b3c8974e592c2fa383d4a3960714caef0c4f2")]
        [InlineData("Slock.it Rockz!", "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514")]
        [InlineData("this is long and very interesting to read.",
            "027d0fd56d418b3b5c21813ddbb0dca5eee817d0c4b64e482f893113b3312bc9")]
        public void ShouldHashStringCorrectly(string plainText, string expectedHash)
        {
            var computedhash = UpdateWatch.HashString(plainText);
            computedhash.Should().Be(expectedHash);
        }


        [Fact]
        public void ShouldStartTimer()
        {
            // Run the test
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "./path",
                DockerComposeControl = new MockDockerControl(),
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            });

            uw.CheckTimer.Should().BeNull();
            
            // start the timer
            uw.StartWatch();

            uw.CheckTimer.Should()
                .NotBeNull();

            // dispose timer
            uw.CheckTimer.Dispose();

        }
        
        [Fact]
        public void ShouldNotUpdateWhenNoNewEvent()
        {
            // Run the test
           
            Mock<IContractWrapper> cwMock = new Mock<IContractWrapper>(MockBehavior.Loose);
            cwMock
                .Setup(mock => mock.HasNewUpdate())
                .Returns(() => Task.FromResult(false))
                .Verifiable("Contract was not checked for update event");
                
            
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "./path",
                DockerComposeControl = new MockDockerControl(),
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = cwMock.Object
            });

            var checkResult = uw.CheckForUpdates(new object());
            checkResult.Should().Be(false);

            cwMock.Verify();
            cwMock.VerifyNoOtherCalls();

        }
        
        [Fact]
        public void ShouldNotUpdateWhenEqualState()
        {
            // Run the test
           
            Mock<IContractWrapper> cwMock = new Mock<IContractWrapper>(MockBehavior.Loose);
            cwMock
                .Setup(mock => mock.HasNewUpdate())
                .Returns(() => Task.FromResult(true))
                .Verifiable("Contract was not checked for update event");
            
            cwMock
                .Setup(mock => mock.GetExpectedState())
                .Returns(() => Task.FromResult(new NodeState
                {
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    ChainspecUrl = "https://example.com",
                    IsSigning = false,
                    ChainspecChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    UpdateIntroducedBlock = new BigInteger(20)
                }))
                .Verifiable("Contract was not queried for state");

            
            // Prime config provider with equal state but older
            MockConfigProvider confMock = new MockConfigProvider
            {
                CurrentState = new NodeState
                {
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    ChainspecUrl = "https://example.com",
                    IsSigning = false,
                    ChainspecChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    UpdateIntroducedBlock = new BigInteger(10)
                }
            };

            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "./path",
                DockerComposeControl = new MockDockerControl(),
                ConfigurationProvider = confMock,
                MessageService = new MockMessageService(),
                ContractWrapper = cwMock.Object
            });

            // Should yield not update actions and therefore should return false
            var checkResult = uw.CheckForUpdates(new object());
            checkResult.Should().Be(false);

            cwMock.Verify();
            cwMock.VerifyNoOtherCalls();

        }
        
         [Fact(Skip = "Takes too long. enable later")]
         
        public void ShouldUpdateWhenDifferentState()
        {
            // Run the test
           
            Mock<IContractWrapper> cwMock = new Mock<IContractWrapper>(MockBehavior.Loose);
            cwMock
                .Setup(mock => mock.HasNewUpdate())
                .Returns(() => Task.FromResult(true))
                .Verifiable("Contract was not checked for update event");
            
            cwMock
                .Setup(mock => mock.GetExpectedState())
                .Returns(() => Task.FromResult(new NodeState
                {
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    ChainspecUrl = "https://example.com",
                    IsSigning = false,
                    ChainspecChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    UpdateIntroducedBlock = new BigInteger(20)
                }))
                .Verifiable("Contract was not queried for state");

            cwMock
                .Setup(mock => mock.ConfirmUpdate())
                .Returns(() => Task.CompletedTask)
                .Verifiable("Contract was not checked for update event");
            
            // Prime config provider with equal state but older
            MockConfigProvider confMock = new MockConfigProvider
            {
                CurrentState = new NodeState
                {
                    DockerImage = "parity/parity:v2.3.5",
                    DockerChecksum = "bbbbcc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    ChainspecUrl = "https://example.com",
                    IsSigning = false,
                    ChainspecChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514",
                    UpdateIntroducedBlock = new BigInteger(10)
                }
            };

            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "./path",
                DockerComposeControl = new MockDockerControl(),
                ConfigurationProvider = confMock,
                MessageService = new MockMessageService(),
                ContractWrapper = cwMock.Object
            });

            // Should yield not update actions and therefore should return false
            var checkResult = uw.CheckForUpdates(new object());
            checkResult.Should().Be(true);

            cwMock.Verify();
            cwMock.VerifyNoOtherCalls();

        }
    }
}