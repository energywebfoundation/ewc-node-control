import {NodeControlSimple} from './contract/NodeControlSimple';
const Web3 = require('web3');


const valAddr = "0xc3681dfe99730eb45154208cba7b0df7e705f305"; // first addr in ganache
console.log("Updating validator: " + valAddr);
const web3 = new Web3('http://localhost:8545');
const contract = new NodeControlSimple(web3, '0x5f51f49e25b2ba1acc779066a2614eb70a9093a0');
contract.updateValidator(valAddr,'0x123456','parity/parity:v2.3.3','0x123456','https://chainspec',true);