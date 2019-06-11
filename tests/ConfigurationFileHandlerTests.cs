using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using FluentAssertions;
using src;
using src.Models;
using Xunit;

namespace tests
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationFileHandlerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void ShouldThrowOnEmptyPath(string path)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                _ = new ConfigurationFileHandler(path);
            });
        }

        [Theory]
        [InlineData("/foobar")]
        [InlineData("./foobar")]
        [InlineData("some-file.foo")]
        public void ShouldThrowOnNonExistingFile(string path)
        {
            Assert.Throws<FileNotFoundException>(() =>
            {
                _ = new ConfigurationFileHandler(path);
            });
        }


        public static IEnumerable<object[]> GenerateReadTestCases()
        {
            // Basic config file
            yield return new object[]
            {
                new List<string>
                {
                    "IS_SIGNING=signing",
                    "PARITY_VERSION=parity/parity:v2.3.3",
                    "PARITY_CHKSUM=9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    "CHAINSPEC_URL=https://example.com/chainspec.json",
                    "CHAINSPEC_CHKSUM=b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"
                },
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    ChainspecUrl = "https://example.com/chainspec.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"
                }
            };

            // Config file with extra key pairs
            yield return new object[]
            {
                new List<string>
                {
                    "IS_SIGNING=signing",
                    "PARITY_VERSION=parity/parity:v2.3.3",
                    "PARITY_CHKSUM=9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    "CHAINSPEC_URL=https://example.com/chainspec.json",
                    "CHAINSPEC_CHKSUM=b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen"

                },
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    ChainspecUrl = "https://example.com/chainspec.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"
                }
            };

            // Config file with extra key pairs and comment
            yield return new object[]
            {
                new List<string>
                {
                    "#This is a config file",
                    "IS_SIGNING=signing",
                    "PARITY_VERSION=parity/parity:v2.3.3",
                    "PARITY_CHKSUM=9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    "CHAINSPEC_URL=https://example.com/chainspec.json",
                    "CHAINSPEC_CHKSUM=b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "This is a comment right in the middle",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen"

                },
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    ChainspecUrl = "https://example.com/chainspec.json",
                    ChainspecChecksum = "b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"
                }
            };

            // Config file with missing key pairs
            yield return new object[]
            {
                new List<string>
                {
                    "#This is a config file",
                    "IS_SIGNING=signing",
                    "PARITY_VERSION=parity/parity:v2.3.3",
                    "PARITY_CHKSUM=9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "This is a comment right in the middle",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen"

                },
                new NodeState
                {
                    IsSigning = true,
                    DockerImage = "parity/parity:v2.3.3",
                    DockerChecksum = "9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    ChainspecUrl = "",
                    ChainspecChecksum =  ""
                }
            };
        }

        public static IEnumerable<object[]> GenerateWriteTestCases()
        {
            // Write new parity version and switch signer w/o chainspec info
            yield return new object[]
            {
                new List<string>
                {
                    "#This is a config file",
                    "IS_SIGNING=signing",
                    "PARITY_VERSION=parity/parity:v2.3.3",
                    "PARITY_CHKSUM=9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "This is a comment right in the middle",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen",
                    "CHAINSPEC_URL=https://example.com/chainspec.json",
                    "CHAINSPEC_CHKSUM=b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"

                },
                new NodeState
                {
                    IsSigning = false,
                    DockerImage = "parity/parity:v2.4.0",
                    DockerChecksum = "30b9c9852b52546e7de32832ddfaa4aba33c0c436c985ad97d3fe9c4210043d8"
                },
                new List<string>
                {
                    "#This is a config file",
                    "IS_SIGNING=non-signing",
                    "PARITY_VERSION=parity/parity:v2.4.0",
                    "PARITY_CHKSUM=30b9c9852b52546e7de32832ddfaa4aba33c0c436c985ad97d3fe9c4210043d8",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "This is a comment right in the middle",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen",
                    "CHAINSPEC_URL=https://example.com/chainspec.json",
                    "CHAINSPEC_CHKSUM=b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"

                }
            };

             // Write new file with all info
            yield return new object[]
            {
                new List<string>
                {
                    "#This is a config file",
                    "IS_SIGNING=signing",
                    "PARITY_VERSION=parity/parity:v2.3.3",
                    "PARITY_CHKSUM=9f0142e1ae1641fbcf6116c49b5c73d5a4b48340e07f9ecb4da8d2d9847a76e6",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "This is a comment right in the middle",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen",
                    "CHAINSPEC_URL=https://example.com/chainspec.json",
                    "CHAINSPEC_CHKSUM=b76377f4f130134f352e81c8929fb0c8ffca94da722f704d16d0873fc9e030ea"

                },
                new NodeState
                {
                    IsSigning = false,
                    DockerImage = "parity/parity:v2.4.0",
                    DockerChecksum = "30b9c9852b52546e7de32832ddfaa4aba33c0c436c985ad97d3fe9c4210043d8",
                    ChainspecUrl = "https://example.com/chainspec-20190303.json",
                    ChainspecChecksum = "7e40a9d4a3066f839882e0fc26acd9e2f08bdbd314b7319db7cae54cd4450ee9"
                },
                new List<string>
                {
                    "#This is a config file",
                    "IS_SIGNING=non-signing",
                    "PARITY_VERSION=parity/parity:v2.4.0",
                    "PARITY_CHKSUM=30b9c9852b52546e7de32832ddfaa4aba33c0c436c985ad97d3fe9c4210043d8",
                    "ANOTHER_KEY=foobar",
                    "KEY_2=awesomesource",
                    "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                    "SHELL=/usr/bin/zsh",
                    "This is a comment right in the middle",
                    "SHLVL=2",
                    "SSH_AGENT_PID=2940",
                    "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                    "TERM=screen",
                    "CHAINSPEC_URL=https://example.com/chainspec-20190303.json",
                    "CHAINSPEC_CHKSUM=7e40a9d4a3066f839882e0fc26acd9e2f08bdbd314b7319db7cae54cd4450ee9"

                }
            };
        }

        [Theory]
        [MemberData(nameof(GenerateReadTestCases))]
        public void ShouldReadFromFile(List<string> fileLines, NodeState state)
        {
            string tmpFilePAth = Path.GetTempFileName();
            File.WriteAllLines(tmpFilePAth,fileLines);
            ConfigurationFileHandler cfh = new ConfigurationFileHandler(tmpFilePAth);
            NodeState resultState = cfh.ReadCurrentState();
            resultState.Should().BeEquivalentTo(state);
        }

        [Theory]
        [MemberData(nameof(GenerateWriteTestCases))]
        public void ShouldWriteToFile(List<string> inputFile, NodeState newState, List<string> expectedLines)
        {
            string tmpFilePAth = Path.GetTempFileName();
            File.WriteAllLines(tmpFilePAth,inputFile);

            ConfigurationFileHandler cfh = new ConfigurationFileHandler(tmpFilePAth);
            cfh.WriteNewState(newState);

            List<string> resultingLines = File.ReadAllLines(tmpFilePAth).ToList();

            resultingLines.Should().ContainInOrder(expectedLines);
        }

        [Fact]
        public void ShouldThrowIfFileDissapears()
        {
            string tmpFilePAth = Path.GetTempFileName();
            File.WriteAllLines(tmpFilePAth, new List<string>
            {
                "#This is a config file",
                "IS_SIGNING=non-signing",
                "PARITY_VERSION=parity/parity:v2.4.0",
                "PARITY_CHKSUM=30b9c9852b52546e7de32832ddfaa4aba33c0c436c985ad97d3fe9c4210043d8",
                "ANOTHER_KEY=foobar",
                "KEY_2=awesomesource",
                "SESSION_MANAGER=local/yoga-markus:@/tmp/.ICE-unix/2876,unix/yoga-markus:/tmp/.ICE-unix/2876",
                "SHELL=/usr/bin/zsh",
                "This is a comment right in the middle",
                "SHLVL=2",
                "SSH_AGENT_PID=2940",
                "SSH_AUTH_SOCK=/run/user/1000/keyring/ssh",
                "TERM=screen",
                "CHAINSPEC_URL=https://example.com/chainspec-20190303.json",
                "CHAINSPEC_CHKSUM=7e40a9d4a3066f839882e0fc26acd9e2f08bdbd314b7319db7cae54cd4450ee9"

            });

            // instantiate with existing file to pass first file check
            ConfigurationFileHandler cfh = new ConfigurationFileHandler(tmpFilePAth);

            // remove file
            File.Delete(tmpFilePAth);

            // make sure it was removed
            if (File.Exists(tmpFilePAth))
            {
                throw new TestFailureException("Test file didn't get removed. Results would be wrong.");
            }

            // Should fail on a file write attempt
            Assert.Throws<FileNotFoundException>(() => { cfh.WriteNewState(new NodeState()); });

            // Should fail on a file read attempt
            Assert.Throws<FileNotFoundException>(() => { _ = cfh.ReadCurrentState(); });

        }

    }
}