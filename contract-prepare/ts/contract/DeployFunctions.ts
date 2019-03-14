
import { TransactionReceipt, Logs } from 'web3/types';
import Web3 from 'web3';
import { SpecialTx } from './GeneralFunctions';
import { Tx } from 'web3/eth/types';
import NodeControlSimpleJSON from './NodeControlSimple.json'


export const deployNodeControlSimple = async(web3: Web3, txParams ?: SpecialTx): Promise<TransactionReceipt> => {

    if ( NodeControlSimpleJSON.bytecode.length === 0) throw new Error('no bytecode provided')

    const byteCode = NodeControlSimpleJSON.bytecode.startsWith('0x')? NodeControlSimpleJSON.bytecode : '0x'+ NodeControlSimpleJSON.bytecode
            
    const transactionbBytecode = byteCode

    const transactionParams: Tx = {
        to: '',
        data: transactionbBytecode
    }

    if(txParams){
        let senderAddress 
        if(txParams.privateKey){
            const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
            senderAddress =  web3.eth.accounts.privateKeyToAccount(pk).address
            if(txParams.from && (txParams.from != senderAddress)) throw new Error ('provided PrivateKey and account do not match')
        } else {
            senderAddress = txParams.from? txParams.from : (await web3.eth.getAccounts())[0]
        }
        transactionParams.from = senderAddress
        transactionParams.gas = txParams.gas? txParams.gas : 7000000
        transactionParams.nonce = txParams.nonce? txParams.nonce : await web3.eth.getTransactionCount(transactionParams.from)
        transactionParams.value = txParams.value ? txParams.value : 0
    } else {
        transactionParams.from =  (await web3.eth.getAccounts())[0]
        transactionParams.gas = 7000000
        transactionParams.nonce = await web3.eth.getTransactionCount(transactionParams.from)
        transactionParams.value= 0
    }

    if(txParams && txParams.privateKey){
        const privateKey = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey
        const signedTx = await web3.eth.accounts.signTransaction(transactionParams, privateKey)
        return(await web3.eth.sendSignedTransaction(signedTx.rawTransaction))
    } else {
        return(await web3.eth.sendTransaction(transactionParams))
    }
        
}
