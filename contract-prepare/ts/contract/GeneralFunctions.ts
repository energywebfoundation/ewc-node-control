
import Web3 from 'web3';
import { Tx, BlockType } from 'web3/eth/types';
import { TransactionReceipt, Logs } from 'web3/types';
import { JsonRPCResponse } from 'web3/providers'
    
export declare interface SpecialTx extends Tx {
    privateKey?: string;
}

export declare interface SearchLog extends Logs {
    toBlock?: number;
}

export async function getClientVersion(web3: Web3): Promise<string> {

    return new Promise<string>((resolve, reject) => {
        (web3.currentProvider as any).send(
            {
                jsonrpc: '2.0',
                method: 'web3_clientVersion',
                params: [],
                id: 1
            },
            (e:any, r:any) => {
                if (e) reject(e);
                else resolve(r.result);
            });
    });
}

export async function replayTransaction(web3: Web3, txHash: string) {
    return new Promise((resolve, reject) => {
        (web3.currentProvider as any).send(
            {
                jsonrpc: '2.0',
                method: 'trace_replayTransaction',
                params: [txHash, ['trace', 'vmTrace', 'stateDiff']],
                id: 1,
            },
            (e:any, r:any) => {
                if (e) reject(e);
                else resolve(r.result);
            });
    });
}

    export async function getErrorMessage(web3: Web3, txParams: SpecialTx): Promise<string> {

    const gasObj = txParams.gas? txParams.gas : 7000000
    const txObj: Tx = {
        from: txParams.from,
        to: txParams.to,
        data: txParams.data,
        value: txParams.value,
        gas: web3.utils.toHex(gasObj)
    }

    return await new Promise<any>((resolve, reject) => {
        (web3.currentProvider as any).send(
            {
                jsonrpc: '2.0',
                method: 'trace_call',
                params: [txObj, ['trace']],
                id: 1,
            },
            (e, r) => {
                if (e) reject(e);
                else {

                    const outputResult = r.result.output;

                    let shorterAsciiCode = '0x' + outputResult.substr(10);

                    if (r.result.output === "0x") {
                        resolve("Bad instruction / revert without reason string")
                    }

                    resolve(web3.utils.toAscii(shorterAsciiCode));
                }
            });
    });
}

export async function handleTx(web3: Web3, txParams: SpecialTx): Promise<TransactionReceipt> {

    console.log("handleTx")

    const transactionParams: Tx = {
        to: txParams.to? txParams.to : '',
        data: txParams.data,
        from: txParams.from,
        nonce: txParams.nonce? txParams.nonce : await web3.eth.getTransactionCount(txParams.from),
        gas: txParams.gas,
        gasPrice: txParams.gasPrice ? txParams.gasPrice : 0,
        value: txParams.value ? txParams.value : 0
    } 

    return(await web3.eth.sendTransaction(transactionParams))  
}

export async function handleRawTx(web3: Web3, txParams: SpecialTx): Promise<TransactionReceipt> {
    console.log("handleRawTx")

    const transactionParams: Tx = {
        to: txParams.to? txParams.to : '',
        data: txParams.data,
        from: txParams.from,
        nonce: txParams.nonce? txParams.nonce : await web3.eth.getTransactionCount(txParams.from),
        gas: txParams.gas,
        gasPrice: txParams.gasPrice ? txParams.gasPrice : 0,
        value: txParams.value ? txParams.value : 0
    } 
    const signedTx = await web3.eth.accounts.signTransaction(transactionParams, txParams.privateKey)
    return(await web3.eth.sendSignedTransaction(signedTx.rawTransaction))
   
}


export class GeneralFunctions {
    web3Contract: any
    constructor(web3Contract) {
        this.web3Contract = web3Contract
    }

    async sendRaw(web3: Web3, privateKey: string, txParams: Tx): Promise<TransactionReceipt> {
        const txData = {
            nonce: txParams.nonce,
            gasLimit: txParams.gas,
            gasPrice: txParams.gasPrice,
            data: txParams.data,
            from: txParams.from,
            to: txParams.to
        }

        const txObject = await web3.eth.accounts.signTransaction(txData, privateKey)
        return (await web3.eth.sendSignedTransaction((txObject as any).rawTransaction))
    }

    getWeb3Contract() {
        return this.web3Contract
    }
}
    