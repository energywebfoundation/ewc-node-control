using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using src;
using src.Models;
using tests.Mocks;
using Xunit;

namespace tests
{

    [ExcludeFromCodeCoverage]
    public class StateCompareTests
    {
        [Fact]
        public void ShouldNotInstantiateWithoutConfigProvider()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                _ = new StateCompare(null);
            });
        }
        
        [Fact]
        public void ShouldThrowWhenComparingNullState()
        {
            MockConfigProvider cp = new MockConfigProvider();
            StateCompare sc = new StateCompare(cp);
            
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => { sc.ComputeActionsFromState(null); });
            Assert.Equal("newState",ex.ParamName);
        }
        
        [Fact]
        public void ShouldThrowWhenNullStateFromConfigProvider()
        {
            MockConfigProvider cp = new MockConfigProvider {CurrentState = null};
            StateCompare sc = new StateCompare(cp);
            
            StateCompareException ex = Assert.Throws<StateCompareException>(() =>
            {
                sc.ComputeActionsFromState(new NodeState());
            });
            
            Assert.Equal("Received state from configuration provider is null. Can't compare",ex.Message);
        }
        
        [Fact]
        public void ShouldGenerateNoActionsOnSameState()
        {
            
            MockConfigProvider cp = new MockConfigProvider {CurrentState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://foo.bar",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            }};
            
            // Recreate the same state so state object references are different
            NodeState newState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://foo.bar",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            };
            
            StateCompare sc = new StateCompare(cp);
            List<StateChangeAction> actions = sc.ComputeActionsFromState(newState);

            // Assert
            actions.Should().BeEmpty();

        }
        
        [Fact]
        public void ShouldGenerateNoActionsOnEqualState()
        {
            // Recreate a single state so state object references are equal
            NodeState sharedState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://foo.bar",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            };
            
            MockConfigProvider cp = new MockConfigProvider {CurrentState = sharedState};
            
            StateCompare sc = new StateCompare(cp);
            List<StateChangeAction> actions = sc.ComputeActionsFromState(sharedState);

            // Assert
            actions.Should().BeEmpty();

        }
        
        [Fact]
        public void ShouldGenerateDockerAction()
        {
            
            MockConfigProvider cp = new MockConfigProvider {CurrentState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://foo.bar",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            }};
            
            // Recreate the same state so state object references are different
            NodeState newState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.4.4",
                DockerChecksum = "c30bcff5580cb5d9c4edb0a0c8794210e85a134c2f12ffd166735e7c26079211",
                ChainspecUrl = "https://foo.bar",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            };
            
            StateCompare sc = new StateCompare(cp);
            List<StateChangeAction> actions = sc.ComputeActionsFromState(newState);

            // Assert
            actions.Should().HaveCount(1);
            actions.Should().ContainSingle(action => 
                action.Mode == UpdateMode.Docker &&
                action.Payload == "parity/parity:v2.4.4" && 
                action.PayloadHash == "c30bcff5580cb5d9c4edb0a0c8794210e85a134c2f12ffd166735e7c26079211");

        }
        
        [Fact]
        public void ShouldGenerateChainspecAction()
        {
            
            MockConfigProvider cp = new MockConfigProvider {CurrentState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://www.example.com/chainspec-20190210.json",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            }};
            
            // Recreate the same state so state object references are different
            NodeState newState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://www.example.com/chainspec-20190215.json",
                ChainspecChecksum = "c30bcff5580cb5d9c4edb0a0c8794210e85a134c2f12ffd166735e7c26079211",
                UpdateIntroducedBlock = 1234567
            };
            
            StateCompare sc = new StateCompare(cp);
            List<StateChangeAction> actions = sc.ComputeActionsFromState(newState);

            // Assert
            actions.Should().HaveCount(1);
            actions.Should().ContainSingle(action => 
                action.Mode == UpdateMode.ChainSpec &&
                action.Payload == "https://www.example.com/chainspec-20190215.json" && 
                action.PayloadHash == "c30bcff5580cb5d9c4edb0a0c8794210e85a134c2f12ffd166735e7c26079211");

        }
        
        [Fact]
        public void ShouldGenerateSigningAction()
        {
            
            MockConfigProvider cp = new MockConfigProvider {CurrentState = new NodeState
            {
                IsSigning = true,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://www.example.com/chainspec-20190210.json",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            }};
            
            // Recreate the same state so state object references are different
            NodeState newState = new NodeState
            {
                IsSigning = false,
                DockerImage = "parity/parity:v2.3.4",
                DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                ChainspecUrl = "https://www.example.com/chainspec-20190210.json",
                ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                UpdateIntroducedBlock = 1234567
            };
            
            StateCompare sc = new StateCompare(cp);
            List<StateChangeAction> actions = sc.ComputeActionsFromState(newState);

            // Assert
            actions.Should().HaveCount(1);
            actions.Should().ContainSingle(action => 
                action.Mode == UpdateMode.ToggleSigning &&
                action.Payload == false.ToString() && 
                action.PayloadHash == string.Empty);

        }

        public static IEnumerable<object[]> GenerateTestCasesForActionCombinations()
        {
            // Test docker and chain
            yield return new object[]
            {
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.3.4",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    ChainspecUrl = "https://www.example.com/chainspec-20190210.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                    UpdateIntroducedBlock = 1234567
                },
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.4.4",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a1111",
                    ChainspecUrl = "https://www.example.com/chainspec-20190213.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e02222",
                    UpdateIntroducedBlock = 1234567
                },
                new List<StateChangeAction>
                {
                    new StateChangeAction
                    {
                        Mode = UpdateMode.Docker,
                        Payload = "parity/parity:v2.4.4",
                        PayloadHash = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a1111"
                    },
                    new StateChangeAction
                    {
                        Mode = UpdateMode.ChainSpec,
                        Payload = "https://www.example.com/chainspec-20190213.json",
                        PayloadHash = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e02222"
                    }
                }
            };
            
            // Test docker, Chain and signing toggle
            yield return new object[]
            {
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.3.4",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    ChainspecUrl = "https://www.example.com/chainspec-20190210.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                    UpdateIntroducedBlock = 1234567
                },
                new NodeState
                {
                    IsSigning = false,
                    DockerImage = "parity/parity:v2.4.4",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a1111",
                    ChainspecUrl = "https://www.example.com/chainspec-20190213.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e02222",
                    UpdateIntroducedBlock = 1234567
                },
                new List<StateChangeAction>
                {
                    new StateChangeAction
                    {
                        Mode = UpdateMode.Docker,
                        Payload = "parity/parity:v2.4.4",
                        PayloadHash = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a1111"
                    },
                    new StateChangeAction
                    {
                        Mode = UpdateMode.ChainSpec,
                        Payload = "https://www.example.com/chainspec-20190213.json",
                        PayloadHash = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e02222"
                    },
                    new StateChangeAction
                    {
                        Mode = UpdateMode.ToggleSigning,
                        Payload = false.ToString(),
                        PayloadHash = string.Empty
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(GenerateTestCasesForActionCombinations))]
        public void ShouldGenerateActionCombinations(NodeState currentState, NodeState newState, List<StateChangeAction> expectedActions)
        {
            
            MockConfigProvider cp = new MockConfigProvider {CurrentState = currentState};
           
            StateCompare sc = new StateCompare(cp);
            List<StateChangeAction> actions = sc.ComputeActionsFromState(newState);

            // Assert
            actions.Should().HaveCount(expectedActions.Count);

            foreach (StateChangeAction expAction in expectedActions)
            {
                actions.Should().ContainEquivalentOf(expAction);
            }
            
        }
    }
}