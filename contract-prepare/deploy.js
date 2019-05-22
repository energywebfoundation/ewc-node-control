const Web3 = require("web3");
const fs = require('fs');
const EthereumTx = require('ethereumjs-tx');

const main = async () => {
    
    const rpc = "http://localhost:8545"
    const pk = "ae29ab491cf53d8b63f281cc5eecdbbac4a992b2a4bf483bacae66dfff0740f0";
    const valAddr = "0xc3681dfe99730eb45154208cba7b0df7e705f305"; // first addr in ganache

    console.log("Deploying to " + rpc);
    const web3 = new Web3(rpc);
    const sendAddr = web3.eth.accounts.privateKeyToAccount(pk).address;

    console.log("Deploying NodeControl Contract...");
    const contractNcJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlSimple.json").toString());
    var ncContract = new web3.eth.Contract(contractNcJson.abi);
    await ncContract.deploy({
        data: '0x' + contractNcJson.bin
    })
    .send({
        from: sendAddr,
        gas: web3.utils.toHex(1000000),
        gasPrice: web3.utils.toHex(0),
    });

    console.log("Deploying Lookup Contract...");
    const contractLookupJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlLookUp.json").toString());
    var lookupContract = new web3.eth.Contract(contractLookupJson.abi);
    await lookupContract.deploy({
        data: '0x' + contractLookupJson.bin,
        arguments: ['0x5f51f49e25b2ba1acc779066a2614eb70a9093a0']
    })
    .send({
        from: sendAddr,
        gas: web3.utils.toHex(1000000),
        gasPrice: web3.utils.toHex(0),
    })
    .on('receipt', (receipt) => {
        console.log("LOOKUP ADDRESS = " + receipt.contractAddress) // contains the new contract address
    });


    console.log("Priming...");
e(json);
    const code = '0x' + contract.bin;

    txParams = {
        from: sendAddr,
        nonce: 1,
        gas: web3.utils.toHex(1000000),
        gasPrice: web3.utils.toHex(0),
        to: '',
        value:0,
        data: code
    };

    const transaction = new EthereumTx(txParams);
    const privateKeyBuffer = Buffer.from(pk, 'hex');
    transaction.sign(privateKeyBuffer);
    const serializedTx = transaction.serialize().toString('hex');
    await web3.eth.sendSignedTransaction('0x' + serializedTx);

    console.log("Priming Contract...");
    
    await ncInstance.methods.updateValidator(valAddr,'0x123456','parity/parity:v2.3.3','0x123456','https://chainspec',true);

    console.log("Done.")
   
}

main()