const Web3 = require("web3");
const fs = require('fs');
const EthereumTx = require('ethereumjs-tx');

const main = async () => {
    
    const rpc = "http://localhost:8545"
    const pk = "ae29ab491cf53d8b63f281cc5eecdbbac4a992b2a4bf483bacae66dfff0740f0";

    console.log("Deploying to " + rpc);
    const web3 = new Web3(rpc);
    const gasPrice = 1000000000; 

    console.log(gasPrice);
    const json = fs.readFileSync("./ts/contract/NodeControlSimple.1.json").toString();
    const contract = JSON.parse(json);
    const code = '0x' + contract.bin;
    const sendAddr = web3.eth.accounts.privateKeyToAccount(pk).address;

    txParams = {
        from: sendAddr,
        nonce: web3.utils.toHex(await web3.eth.getTransactionCount(sendAddr)),
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
    return (await web3.eth.sendSignedTransaction('0x' + serializedTx));

   
}

main()