const Sloffle = require("sloffle")
const Web3 = require("web3");
const fs = require('fs');

const main = async () => {
    
    const rpc = "http://localhost:8545"
    const pk = "0xae29ab491cf53d8b63f281cc5eecdbbac4a992b2a4bf483bacae66dfff0740f0";

    console.log("Deploying to " + rpc);
    const web3 = new Web3(rpc);
    const gasPrice = 1000000000; 

    console.log(gasPrice);
    const json = fs.readFileSync("./ts/contract/NodeControlSimple.json").toString();
    const foo = await Sloffle.deploy(web3,JSON.parse(json),[]
    ,{privateKey:pk,gasPrice:gasPrice})
    console.log(foo);
}

main()