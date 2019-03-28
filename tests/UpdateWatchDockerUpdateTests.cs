using System;
using System.Collections.Generic;
using Docker.DotNet.Models;
using FluentAssertions;
using Moq;
using src;
using src.Interfaces;
using src.Models;
using tests.Mocks;
using Xunit;

namespace tests
{
    public class UpdateWatchDockerUpdateTests
    {
        public static IEnumerable<object[]> UpateDockerActionShouldNotVerifyCases()
        {
            // Empty payload
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.Docker,
                    Payload = "",
                    PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                new NodeState
                {
                    DockerImage  = "",
                    DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                      
                },
                "Payload or hash are empty"
            };
            
            // Empty payload hash
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.Docker,
                    Payload = "parity/parity:v2.3.3",
                    PayloadHash = ""
                },
                new NodeState
                {
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = ""
                },
                "Payload or hash are empty"
            };
            
            // node state mismatch on docker image
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.Docker,
                    Payload = "parity/parity:v2.3.3",
                    PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                new NodeState
                {
                    DockerImage = "parity/parity:v2.2.3",
                    DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                "Action vs. nodestate mismatch"
            };
            
            // node state mismatch on docker image hash
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.Docker,
                    Payload = "parity/parity:v2.3.3",
                    PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                new NodeState
                {
                    DockerImage = "parity/parity:v2.2.3",
                    DockerChecksum = "bbbbcc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                "Action vs. nodestate mismatch"
            };
            
            // bad action mode
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.ChainSpec,
                    Payload = "parity/parity:v2.3.3",
                    PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                new NodeState
                {
                    DockerImage = "parity/parity:v2.2.3",
                    DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                },
                "Action with wrong update mode passed"
            };
        }

        [Theory]
        [MemberData(nameof(UpateDockerActionShouldNotVerifyCases))]
        public void UpateChainSpecActionShouldNotVerify(StateChangeAction changeAction,NodeState state, string expectedMessage)
        {
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "/some/path",
                DockerControl = new MockDockerControl(),
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            }, new MockLogger());

            Action updateDocker = () => { uw.UpdateDocker(changeAction,state); };
            updateDocker.Should()
                .Throw<UpdateVerificationException>()
                .WithMessage(expectedMessage);

        }

        [Fact]
        public void UpdateDockerShouldUpdate()
        {

            string expectedImage = "parity/parity:v2.3.4";
            string expectedHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514";

            var changeAction = new StateChangeAction
            {
                Mode = UpdateMode.Docker,
                Payload = "parity/parity:v2.3.4",
                PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
            };
            var nodeState = new NodeState
            {
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
            };
            
            
            Mock<IDockerControl> mocDcc = new Mock<IDockerControl>(MockBehavior.Loose);
            
            // Setup image pull mock 
            mocDcc.Setup(mock => mock.PullImage(
                    It.Is<ImagesCreateParameters>(icp => icp.Tag == "v2.3.4" && icp.FromImage == "parity/parity"),
                    It.Is<AuthConfig>(obj => obj == null),
                    It.IsAny<Progress<JSONMessage>>()))
                .Verifiable("Did not pull correct image");
            
            // Setup inspect image mock
            mocDcc
                .Setup(mock => mock.InspectImage(
                    It.Is<string>(i => i == expectedImage)
                ))
                .Returns(new ImageInspectResponse
                {
                    ID = expectedHash
                })
                .Verifiable("Did not inspect the correct image");

            // setup apply mock
            mocDcc
                .Setup(mock => mock.ApplyChangesToStack("/some/path",false))
                .Verifiable("Did not correctly apply the changes to the stack");
            
            MockConfigProvider confProvider = new MockConfigProvider();
            
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "/some/path",
                DockerControl = mocDcc.Object,
                ConfigurationProvider = confProvider,
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            }, new MockLogger());

            // Run the update action
            Action updateDocker = () => { uw.UpdateDocker(changeAction,nodeState); };
            updateDocker.Should()
                .NotThrow();
            
            // Verify the mocks
            mocDcc.Verify();
            
            // make sure nothing else was called
            mocDcc.VerifyNoOtherCalls();
            
            // verify the newly written state on the config provider
            confProvider.CurrentState.Should().BeEquivalentTo(nodeState);
        }
        
        [Fact]
        public void ShoudThrowOnBadHash()
        {

            string expectedImage = "parity/parity:v2.3.4";
            string expectedHash = "bbbbcc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514";

            var changeAction = new StateChangeAction
            {
                Mode = UpdateMode.Docker,
                Payload = "parity/parity:v2.3.4",
                PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
            };
            var nodeState = new NodeState
            {
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
            };
            
            
            Mock<IDockerControl> mocDcc = new Mock<IDockerControl>(MockBehavior.Loose);
            
            // Setup image pull mock 
            mocDcc.Setup(mock => mock.PullImage(
                    It.Is<ImagesCreateParameters>(icp => icp.Tag == "v2.3.4" && icp.FromImage == "parity/parity"),
                    It.Is<AuthConfig>(obj => obj == null),
                    It.IsAny<Progress<JSONMessage>>()))
                .Verifiable("Did not pull correct image");
            
            // Setup inspect image mock
            mocDcc
                .Setup(mock => mock.InspectImage(
                    It.Is<string>(i => i == expectedImage)
                ))
                .Returns(new ImageInspectResponse
                {
                    ID = expectedHash
                })
                .Verifiable("Did not inspect the correct image");


            // setup delete mock
            mocDcc
                .Setup(mock => mock.DeleteImage(expectedImage))
                .Verifiable("Did not correctly delete the wrong image");
            
            MockConfigProvider confProvider = new MockConfigProvider();
            
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "/some/path",
                DockerControl = mocDcc.Object,
                ConfigurationProvider = confProvider,
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            }, new MockLogger());

            // Run the update action
            Action updateDocker = () => { uw.UpdateDocker(changeAction,nodeState); };
            updateDocker.Should()
                .Throw<UpdateVerificationException>()
                .WithMessage("Docker image hashes don't match.");
            
            // Verify the mocks
            mocDcc.Verify();
            
            // make sure nothing else was called
            mocDcc.VerifyNoOtherCalls();
            
            // verify no new state was written
            confProvider.CurrentState.Should().BeNull();
        }
        
        [Fact]
        public void ShoudThrowOnUnableToPull()
        {

            var changeAction = new StateChangeAction
            {
                Mode = UpdateMode.Docker,
                Payload = "parity/parity:v2.3.4",
                PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
            };
            var nodeState = new NodeState
            {
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
            };
            
            
            Mock<IDockerControl> mocDcc = new Mock<IDockerControl>(MockBehavior.Loose);
            
            // Setup image pull mock 
            mocDcc.Setup(mock => mock.PullImage(
                    It.IsAny<ImagesCreateParameters>(),
                    It.IsAny<AuthConfig>(),
                    It.IsAny<Progress<JSONMessage>>()))
                .Throws<Exception>()
                .Verifiable("Did not pull correct image");
            
            
            MockConfigProvider confProvider = new MockConfigProvider();
            
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "/some/path",
                DockerControl = mocDcc.Object,
                ConfigurationProvider = confProvider,
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            }, new MockLogger());

            // Run the update action
            Action updateDocker = () => { uw.UpdateDocker(changeAction,nodeState); };
            updateDocker.Should()
                .Throw<UpdateVerificationException>()
                .WithMessage("Unable to pull new image.");
            
            // Verify the mocks
            mocDcc.Verify();
            
            // make sure nothing else was called
            mocDcc.VerifyNoOtherCalls();
            
            // verify no new state was written
            confProvider.CurrentState.Should().BeNull();
        }
        
    }
}