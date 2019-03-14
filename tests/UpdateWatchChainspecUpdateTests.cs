using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using src;
using src.Models;
using tests.Mocks;
using Xunit;

namespace tests
{
    public class UpdateWatchChainspecUpdateTests
    {
        public static IEnumerable<object[]> UpateChainSpecActionShouldNotVerifyCases()
        {
            // Empty payload
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.ChainSpec,
                    Payload = "",
                    PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                }, "Payload or hash are empty"
            };
            
            // Empty payload hash
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.ChainSpec,
                    Payload = "https://example.com",
                    PayloadHash = ""
                }, "Payload or hash are empty"
            };
            
            // Non https url
            yield return new object[]
            {
                new StateChangeAction
                {
                    Mode = UpdateMode.ChainSpec,
                    Payload = "http://example.com",
                    PayloadHash = "a783cc3d9b971ea268eb723eb8c653519f39abfa3d6819c1ee1f0292970cf514"
                }, "Won't download chainspec from unencrypted URL"
            };
        }

        [Theory]
        [MemberData(nameof(UpateChainSpecActionShouldNotVerifyCases))]
        public void UpateChainSpecActionShouldNotVerify(StateChangeAction badState, string expectedMessage)
        {
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = "/some/path",
                DockerComposeControl = new MockDockerControl(),
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            });

            Action updateChainSpec = () => { uw.UpdateChainSpec(badState); };
            updateChainSpec.Should()
                .Throw<UpdateVerificationException>()
                .WithMessage(expectedMessage);

        }

        [Fact]
        public void ShouldDownloadChainspec()
        {

            // Test setup 
            string expectedUrl = "https://example.com/chain.json";
            string expectedPayload = "This would be a complete chainspec file.";
            string expectedHash = "8394d0987bd84c677122872aa67f60295b972eceb3f75bec068e83570d3c6999";
            
            // prepare directory
            string basePath = $"nodecontrol-tests-{Guid.NewGuid()}";
            string path = Path.Join(Path.GetTempPath(),basePath);

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Join(path,"config"));
            
            // write existing dummy chainspec
            File.WriteAllText(Path.Join(path,"config","chainspec.json"),"This is not a chainspec file");
            
            // get a mock dcc
            MockDockerControl mockDcc = new MockDockerControl();
            
            StateChangeAction sca = new StateChangeAction
            {
                Mode = UpdateMode.ChainSpec,
                Payload = expectedUrl,
                PayloadHash = expectedHash
            };
            
            
            bool urlCorrect = false;
            // setup mock http handler
            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    // make sure queried url is correct
                    urlCorrect = request.RequestUri.ToString() == expectedUrl;

                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(expectedPayload);
                    return response;
                })
                .Verifiable();
            
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup(
                    "Dispose",
                    ItExpr.IsAny<bool>()
                )
                .Verifiable();
            
            
            // Run the test
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = path,
                DockerComposeControl = mockDcc,
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            });
            
            Action update = () =>
            {
                uw.UpdateChainSpec(sca, handlerMock.Object);
            };
            
            // Execute test
            update.Should().NotThrow();
            
            
            // Should have create a backup file
            
            // Should have written the new payload to file
            string newFileContents = File.ReadAllText(Path.Join(path, "config", "chainspec.json"));
            newFileContents.Should().Be(expectedPayload);
            
            // should have called with the correct url
            urlCorrect.Should().Be(true);
            
            // Should have triggered an update
            mockDcc.ApplyChangesCallCount.Should().Be(1);
            mockDcc.SendRestartOnly.Should().Be(true);
            mockDcc.SendPathToStack.Should().Be(path);
            
            // http client should have been disposed
            Action verifyDispose = () => handlerMock.VerifyAll();
            verifyDispose.Should().NotThrow();
        }
        
        [Fact]
        public void ShouldFailtoVerifyOnBadHash()
        {
            // Test setup 
            string expectedUrl = "https://example.com/chain.json";
            string expectedPayload = "This would be a corrupt chainspec file.";
            string expectedHash = "8394d0987bd84c677122872aa67f60295b972eceb3f75bec068e83570d3c6999";
            
            // prepare directory
            string basePath = $"nodecontrol-tests-{Guid.NewGuid()}";
            string path = Path.Join(Path.GetTempPath(),basePath);

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Join(path,"config"));
            
            // write existing dummy chainspec
            string existingChainSpecContent = "This is the existing chainspec file before update";
            File.WriteAllText(Path.Join(path,"config","chainspec.json"),existingChainSpecContent);
            
            // get a mock dcc
            MockDockerControl mockDcc = new MockDockerControl();
            
            StateChangeAction sca = new StateChangeAction
            {
                Mode = UpdateMode.ChainSpec,
                Payload = expectedUrl,
                PayloadHash = expectedHash
            };
            
            // setup mock http handler
            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) 
                    => new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(expectedPayload)
                    })
                .Verifiable();
            
            // Run the test
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = path,
                DockerComposeControl = mockDcc,
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            });
            
            Action update = () =>
            {
                uw.UpdateChainSpec(sca, handlerMock.Object);
            };
            
            // Execute test
            update.Should().Throw<UpdateVerificationException>()
                .WithMessage("Downloaded chainspec don't matches hash from chain");
            
            // should not call apply updates
            mockDcc.ApplyChangesCallCount.Should().Be(0);
            
            // should not touch current chainspec
            string currentChainspecFileContents = File.ReadAllText(Path.Join(path,"config","chainspec.json"));
            currentChainspecFileContents.Should().Be(existingChainSpecContent);
        }
        
        [Fact]
        public void ShouldFailtoVerifyWhenUnableToDownload()
        {
            // Test setup 
            string expectedUrl = "https://example.com/chain.json";
            string expectedPayload = "This would be a corrupt chainspec file.";
            string expectedHash = "8394d0987bd84c677122872aa67f60295b972eceb3f75bec068e83570d3c6999";
            
            // prepare directory
            string basePath = $"nodecontrol-tests-{Guid.NewGuid()}";
            string path = Path.Join(Path.GetTempPath(),basePath);

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Join(path,"config"));
            
            // write existing dummy chainspec
            string existingChainSpecContent = "This is the existing chainspec file before update";
            File.WriteAllText(Path.Join(path,"config","chainspec.json"),existingChainSpecContent);
            
            // get a mock dcc
            MockDockerControl mockDcc = new MockDockerControl();
            
            StateChangeAction sca = new StateChangeAction
            {
                Mode = UpdateMode.ChainSpec,
                Payload = expectedUrl,
                PayloadHash = expectedHash
            };
            
            // setup mock http handler
            Mock<HttpMessageHandler> handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync((HttpRequestMessage request, CancellationToken cancellationToken) 
                    => new HttpResponseMessage(HttpStatusCode.InternalServerError)
                    {
                        Content = new StringContent("error")
                    })
                .Verifiable();
            
            // Run the test
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = path,
                DockerComposeControl = mockDcc,
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            });
            
            Action update = () =>
            {
                uw.UpdateChainSpec(sca, handlerMock.Object);
            };
            
            // Execute test
            update.Should().Throw<UpdateVerificationException>()
                .WithMessage("Unable to download new chainspec")
                .WithInnerException<AggregateException>();
                
            
            // should not call apply updates
            mockDcc.ApplyChangesCallCount.Should().Be(0);
            
            // should not touch current chainspec
            string currentChainspecFileContents = File.ReadAllText(Path.Join(path,"config","chainspec.json"));
            currentChainspecFileContents.Should().Be(existingChainSpecContent);


        }
        
        [Fact]
        public void ShouldFailtoVerifyWhenCurrentChainspecIsMissing()
        {
            // Test setup 
            string expectedUrl = "https://example.com/chain.json";
            string expectedPayload = "This would be a corrupt chainspec file.";
            string expectedHash = "8394d0987bd84c677122872aa67f60295b972eceb3f75bec068e83570d3c6999";
            
            // prepare directory
            string basePath = $"nodecontrol-tests-{Guid.NewGuid()}";
            string path = Path.Join(Path.GetTempPath(),basePath);

            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Join(path,"config"));

            // get a mock dcc
            MockDockerControl mockDcc = new MockDockerControl();
            
            StateChangeAction sca = new StateChangeAction
            {
                Mode = UpdateMode.ChainSpec,
                Payload = expectedUrl,
                PayloadHash = expectedHash
            };
            
            // Run the test
            UpdateWatch uw = new UpdateWatch(new UpdateWatchOptions
            {
                RpcEndpoint = "http://example.com",
                ContractAddress = "0x0",
                ValidatorAddress = "0x0",
                DockerStackPath = path,
                DockerComposeControl = mockDcc,
                ConfigurationProvider = new MockConfigProvider(),
                MessageService = new MockMessageService(),
                ContractWrapper = new MockContractWrapper()
            });
            
            Action update = () =>
            {
                uw.UpdateChainSpec(sca);
            };
            
            // Execute test
            update.Should().Throw<UpdateVerificationException>()
                .WithMessage("Unable to read current chainspec");
            
            // should not call apply updates
            mockDcc.ApplyChangesCallCount.Should().Be(0);

        }
        
    }
}