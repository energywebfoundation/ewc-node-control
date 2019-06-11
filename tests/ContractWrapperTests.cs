using System;
using System.IO;
using System.Net.Http;
using FluentAssertions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using src.Contract;
using src.Interfaces;
using src.Models;
using tests.Mocks;
using Xunit;

namespace tests
{

    public class ContractWrapperTests
    {

        private static readonly string _keyjson = "{\"id\":\"c8252fc0-49a1-e126-e4b2-44ec61288045\",\"version\":3,\"crypto\":{\"cipher\":\"aes-128-ctr\",\"cipherparams\":{\"iv\":\"3065dd30bfec52cb52c91e6b4e6e5dbb\"},\"ciphertext\":\"c547e030bbd3a66c27cb47a7d679d495e9c53e631b0c28b2fd3385c12dc01c8e\",\"kdf\":\"pbkdf2\",\"kdfparams\":{\"c\":10240,\"dklen\":32,\"prf\":\"hmac-sha256\",\"salt\":\"4ddd56a4e1faf523d76a25b794eda1629741b42bc7049f799b7fb09b9d22cd89\"},\"mac\":\"85fec90f828d311cf3964ac29824de355bbf310e638faa8b937b26d3c281f39a\"},\"address\":\"c3681dfe99730eb45154208cba7b0df7e705f305\",\"name\":\"\",\"meta\":\"{}\"}";

        /* NOTE: These tests require a running and primed ganache. see contract-prepare. Not ready in CI yet */

        private void ResetToSnapshot(string rpc)
        {
            using (HttpClient hc = new HttpClient())
            {
                // Revert ganach to defined snapshot
                hc.PostAsync(rpc,
                    new StringContent(
                        "{ \"method\": \"evm_revert\", \"params\": [1], \"id\": 1, \"jsonrpc\": \"2.0\" }")).Wait();

                // re-snapshot right away so others can do it also
                hc.PostAsync(rpc,
                    new StringContent(
                        "{ \"method\": \"evm_snapshot\", \"params\": [], \"id\": 1, \"jsonrpc\": \"2.0\" }")).Wait();
            }
        }

        //[Fact(Skip = "CI not ready")]
        [Fact]
        public void ShouldQueryContract()
        {
            string contractAddress = "0xa454963c7a6dcbdcd0d3fb281f4e67262fb71586";
            string rpc = Environment.GetEnvironmentVariable("TEST_RPC") ?? "http://localhost:8545";
            string validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";
            string fileToPersistBlockNum = Path.GetTempFileName();

            ResetToSnapshot(rpc);

            IContractWrapper cw = new ContractWrapper(contractAddress,rpc,validatorAddress,new MockLogger(),"test",_keyjson,fileToPersistBlockNum);
            NodeState state = cw.GetExpectedState().Result;
            Assert.Equal("parity/parity:v2.3.3",state.DockerImage);
        }

        //[Fact(Skip = "CI not ready")]
        [Fact]
        public void ShouldBeAbleToConfirmUpdate()
        {
            string contractAddress = "0xa454963c7a6dcbdcd0d3fb281f4e67262fb71586";
            string rpc = Environment.GetEnvironmentVariable("TEST_RPC") ?? "http://localhost:8545";
            string validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";
            string fileToPersistBlockNum = Path.GetTempFileName();


            ResetToSnapshot(rpc);

            IContractWrapper cw = new ContractWrapper(contractAddress,rpc,validatorAddress,new MockLogger(),"test",_keyjson,fileToPersistBlockNum);
            cw.ConfirmUpdate().Wait();

        }

        [Fact(Skip = "No idea how to trigger the exception")]
        public void ShouldThrowOnWrongContractDuringConfirmUpdate()
        {
            string contractAddress = "0xa454963c7a6dcbdcd0d3fb281f4e67262fb71586";
            string rpc = Environment.GetEnvironmentVariable("TEST_RPC") ?? "http://localhost:8545";
            string validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";
            string fileToPersistBlockNum = Path.GetTempFileName();

            ResetToSnapshot(rpc);

            IContractWrapper cw = new ContractWrapper(contractAddress,rpc,validatorAddress, new MockLogger(),"test",_keyjson,fileToPersistBlockNum);

            Action confirmUpdateAction = () => { cw.ConfirmUpdate().Wait(); };
            confirmUpdateAction.Should()
                .Throw<ContractException>()
                .WithMessage("Unable to confirm update");

        }


        // Declare the add update function for the contract
        [Function("updateValidator")]
        public class UpdateValidatorFunction : FunctionMessage
        {
            [Parameter("address", "_targetValidator", 1, true)]
            public string ValidatorAddress { get; set; }

            [Parameter("bytes", "_dockerSha", 2, true)]
            public byte[] DockerSha { get; set; }

            [Parameter("string", "_dockerName", 3, true)]
            public string DockerName { get; set; }

            [Parameter("bytes", "_chainSpecSha", 4, true)]
            public byte[] ChainspecSha { get; set; }

            [Parameter("string", "_chainSpecUrl", 5, true)]
            public string ChainspecUrl { get; set; }

            [Parameter("bool", "_isSigning", 6, true)]
            public bool IsSigning { get; set; }
        }

        //[Fact(Skip = "CI not ready")]
        [Fact]
        public void ShouldCheckForNewupdate()
        {

            string lookupContractAddress = "0xa454963c7a6dcbdcd0d3fb281f4e67262fb71586";
            string ncContractAddress = "0x5f51f49e25b2ba1acc779066a2614eb70a9093a0";
            string rpc = Environment.GetEnvironmentVariable("TEST_RPC") ?? "http://localhost:8545";
            string validatorAddress = "0xc3681dfe99730eb45154208cba7b0df7e705f305";
            string fileToPersistBlockNum = Path.GetTempFileName();

            ResetToSnapshot(rpc);

            // no new update should be seen
            ContractWrapper cw = new ContractWrapper(lookupContractAddress,rpc,validatorAddress, new MockLogger(),"test",_keyjson,fileToPersistBlockNum);
            bool hasUpdate = cw.HasNewUpdate().Result;
            hasUpdate.Should().Be(false);


            // Send an update
            // prepare RPC connection to play some tx

            string contractOwnerPk = "ae29ab491cf53d8b63f281cc5eecdbbac4a992b2a4bf483bacae66dfff0740f0";
            Account account = new Account(contractOwnerPk);

            // create a web 3 instance
            Web3 web3 = new Web3(account,rpc);

            // hook up to the contract and event
            ContractHandler contractHandler = web3.Eth.GetContractHandler(ncContractAddress);

            // contract gets primed with by ganache start
            // const valAddr = "0xc3681dfe99730eb45154208cba7b0df7e705f305"; // first addr in ganache
            // contract.updateValidator(valAddr, '0x123456', 'parity/parity:v2.3.3', '0x123456', 'https://chainspec', true);

            TransactionReceipt confirmResponse =  contractHandler.SendRequestAndWaitForReceiptAsync(new UpdateValidatorFunction
            {
                DockerSha = new byte[]{ 0x0, 0x1, 0x2, 0x3, 0x23 },
                DockerName = "parity/parity:v2.3.4",
                ChainspecSha= new byte[]{ 0x0, 0x1, 0x2, 0x3, 0x23 },
                ChainspecUrl= "https://example.com" + new Random().Next(),
                IsSigning = true,
                ValidatorAddress = validatorAddress
            }).Result;


            bool? hasErrors = confirmResponse.HasErrors();
            if (hasErrors.HasValue && hasErrors.Value)
            {
                throw new ContractException("Unable to confirm update");
            }

            // now an update should be seen
            bool hasUpdate2Nd = cw.HasNewUpdate().Result;
            hasUpdate2Nd.Should().Be(true);

        }
    }
}
