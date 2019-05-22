const Web3 = require("web3");
const fs = require('fs');
const EthereumTx = require('ethereumjs-tx');

const main = async () => {
    
    const rpc = "http://localhost:8545"
    const pk = "ae29ab491cf53d8b63f281cc5eecdbbac4a992b2a4bf483bacae66dfff0740f0";
    const valAddr = "0xc3681dfe99730eb45154208cba7b0df7e705f305"; 

    const privateKeyBuffer = Buffer.from(pk, 'hex');

    console.log("Deploying to " + rpc);
    const web3 = new Web3(rpc);
    const sendAddr = web3.eth.accounts.privateKeyToAccount(pk).address;

    console.log("Deploying NodeControl Contract...");
    const contractNcJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlSimple.json").toString());

    const transaction = new EthereumTx({
        from: sendAddr,
        nonce: 0,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: '',
        value:0,
        data: '0x' + contractNcJson.bytecode
    });
    transaction.sign(privateKeyBuffer);
    const serializedTx = transaction.serialize().toString('hex');
    await web3.eth.sendSignedTransaction('0x' + serializedTx);

    // Deploy lookup
    console.log("Deploying Lookup Contract...");
    const contractLookupJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlLookUp.json").toString());
    const data = web3.eth.abi.encodeParameter('address', '0x5f51f49e25b2ba1acc779066a2614eb70a9093a0').substr(2);

    const transaction2 = new EthereumTx({
        from: sendAddr,
        nonce: 1,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: '',
        value:0,
        data: '0x' + contractLookupJson.bytecode + data
    });
    transaction2.sign(privateKeyBuffer);
    const serializedTx2 = transaction2.serialize().toString('hex');
    await web3.eth.sendSignedTransaction('0x' + serializedTx2);


    console.log("Priming...");

    const ncInstance = new web3.eth.Contract(contractNcJson.abi,"0x5f51f49e25b2ba1acc779066a2614eb70a9093a0");
    

    console.log("Priming Contract...");
    
    const primeData = await ncInstance.methods.updateValidator(
        valAddr,'0x123456','parity/parity:v2.3.3','0x123456','https://chainspec',true).encodeABI();
    const transaction3 = new EthereumTx({
        from: sendAddr,
        nonce: 2,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: "0x5f51f49e25b2ba1acc779066a2614eb70a9093a0",
        value:0,
        data: primeData
    });
    transaction3.sign(privateKeyBuffer);
    const serializedTx3 = transaction3.serialize().toString('hex');
    await web3.eth.sendSignedTransaction('0x' + serializedTx3);
    console.log("Done.")
   
}

main()