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
    const sendAddr = web3.eth.accounts.privateKeyToAccount('0x'+pk).address;
    console.log("Talking from " + sendAddr);

    const addrNc = "0x5f51f49e25b2ba1acc779066a2614eb70a9093a0";
    const addrLookup = "0xa454963c7a6dcbdcd0d3fb281f4e67262fb71586";
    const addrDb = "0xeb8312cf5d2fb55bb131ded00a7adde1ed53860a";

    // deploy nc 
    console.log("Deploy NodeControl...");
    let params = web3.eth.abi.encodeParameters(['address','address'],[addrDb, sendAddr]).substr(2);
    const ncJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlSimple.json").toString());
    let tx = new EthereumTx({
        from: sendAddr,
        nonce: 0,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: '',
        value:0,
        data: '0x' + ncJson.bytecode + params
    });
    
    tx.sign(privateKeyBuffer);
    await web3.eth.sendSignedTransaction('0x' + tx.serialize().toString('hex'));

    // Deploy lookup
    console.log("Deploying Lookup Contract...");
    const contractLookupJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlLookUp.json").toString());
    params = web3.eth.abi.encodeParameters(['address','address'],[addrNc, sendAddr]).substr(2);
    
    tx = new EthereumTx({
        from: sendAddr,
        nonce: 1,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: '',
        value:0,
        data: '0x' + contractLookupJson.bytecode + params
    });
    
    tx.sign(privateKeyBuffer);
    await web3.eth.sendSignedTransaction('0x' + tx.serialize().toString('hex'));
   
   

    console.log("Deploy NodeControl DB...");
    params = web3.eth.abi.encodeParameters(['address','address'],[addrLookup, sendAddr]).substr(2);
    const dbJson = JSON.parse(fs.readFileSync("./contract-build/NodeControlDb.json").toString());
    tx = new EthereumTx({
        from: sendAddr,
        nonce: 2,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: '',
        value:0,
        data: '0x' + dbJson.bytecode + params
    });
    
    tx.sign(privateKeyBuffer);
    await web3.eth.sendSignedTransaction('0x' + tx.serialize().toString('hex'));

   
   

    console.log("Priming Contract...");
    const ncInstance = new web3.eth.Contract(ncJson.abi,addrNc);
    const primeData = await ncInstance.methods.updateValidator(
        valAddr,'0x123456','parity/parity:v2.3.3','0x123456','https://chainspec',true).encodeABI();
    const transaction3 = new EthereumTx({
        from: sendAddr,
        nonce: 3,
        gas: web3.utils.toHex(8000000),
        gasPrice: web3.utils.toHex(0),
        to: "0x5f51f49e25b2ba1acc779066a2614eb70a9093a0",
        value:0,
        data: primeData
    });
    transaction3.sign(privateKeyBuffer);
    const serializedTx3 = transaction3.serialize().toString('hex');
    await web3.eth.sendSignedTransaction('0x' + serializedTx3);
    
    web3.currentProvider.send({ "method": "evm_mine", "params": [], "id": 1, "jsonrpc": "2.0" },() => {
        web3.currentProvider.send({ "method": "evm_snapshot", "params": [], "id": 1, "jsonrpc": "2.0" },() => {
            console.log("Done.")
        });
    });

}

main()