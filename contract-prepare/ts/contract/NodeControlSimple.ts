
import { GeneralFunctions, SpecialTx, SearchLog, getClientVersion, getErrorMessage, handleTx, handleRawTx  } from './GeneralFunctions'
import Web3 from 'web3';
import { Tx, BlockType } from 'web3/eth/types';
import { TransactionReceipt, Logs } from 'web3/types';
import { JsonRPCResponse } from 'web3/providers';
import NodeControlSimpleJSON from './NodeControlSimple.json'
    
export class NodeControlSimple extends GeneralFunctions{
    
    web3: Web3;
    buildFile = NodeControlSimpleJSON;
    constructor(web3: Web3, address?: string){
        super(
            address ? 
                new web3.eth.Contract(NodeControlSimpleJSON.abi, address) : 
                new web3.eth.Contract(NodeControlSimpleJSON.abi, null)
            )
        this.web3 = web3
    }
            
    async getAllUpdateAvailableEvents(eventFilter?:SearchLog){
        let filterParams
        if(eventFilter){
            filterParams = {
                fromBlock: eventFilter.fromBlock? eventFilter.fromBlock: 0, 
                toBlock: eventFilter.toBlock? eventFilter.toBlock: 'latest'
            }
            if (eventFilter.topics) {
                filterParams.topics = eventFilter.topics;
            }
        } else {
            filterParams = {
                fromBlock:0,
                toBlock:'latest' 
            }
        }
        return await this.web3Contract.getPastEvents('UpdateAvailable', filterParams)
    }
            
    async getAllEvents(eventFilter?:SearchLog){
        let filterParams
        if(eventFilter){
            filterParams = {
                fromBlock: eventFilter.fromBlock? eventFilter.fromBlock: 0,
                toBlock: eventFilter.toBlock? eventFilter.toBlock: 'latest',
                topics: eventFilter.topics? eventFilter.topics: [null]
            }
        } else {
            filterParams = {
                fromBlock:0,
                toBlock:'latest',
                topics:[null]
            }
        }
        return await this.web3Contract.getPastEvents('allEvents', filterParams)
    }


	async updateValidator(_targetValidator:string,_dockerSha:string,_dockerName:string,_chainSpecSha:string,_chainSpecUrl:string,_isSigning:boolean, txParams?:SpecialTx): Promise<TransactionReceipt> {

        const transactionParams: SpecialTx = {}

        transactionParams.gas = (txParams && txParams.gas)? txParams.gas : 0 

        transactionParams.to = this.web3Contract._address

        const txData = await this.web3Contract.methods.updateValidator(_targetValidator,_dockerSha,_dockerName,_chainSpecSha,_chainSpecUrl,_isSigning)
            .encodeABI()

        transactionParams.data = txData

        if(!txParams) txParams = {}

        if(txParams.from) {
            transactionParams.from = txParams.from
        }

        if(txParams.value) {
            transactionParams.value = txParams.value
        }

        if(txParams.nonce){
            transactionParams.nonce = txParams.nonce
        }

        if(txParams.privateKey){

            const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
            const senderAddress =  this.web3.eth.accounts.privateKeyToAccount(pk).address
            transactionParams.privateKey = pk

            if(txParams.from) {
                if(txParams.from != senderAddress) throw new Error ('provided PrivateKey and account do not match')
            }
            transactionParams.from = senderAddress
        }

        if(!transactionParams.from) {
            const web3accounts =  await this.web3.eth.getAccounts()
            if(web3accounts.length == 0)throw new Error ("no ethereum account available")

            const web3acc = web3accounts[0]
            if(!web3acc && !transactionParams.privateKey) throw new Error ("no ethereum account available")

            transactionParams.from = web3acc
        }

        if(transactionParams.gas == 0){

            try {
                let gas = await this.web3Contract.methods.updateValidator(_targetValidator,_dockerSha,_dockerName,_chainSpecSha,_chainSpecUrl,_isSigning)
                    .estimateGas({ 
                        from: transactionParams.from,
                        value: transactionParams.value
                    })

                 gas = Math.round(gas*2)
                transactionParams.gas = gas 
            } catch(ex) {
                if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
                const errorResult = await getErrorMessage(this.web3, transactionParams)
                throw new Error(errorResult);
            }
        }

        try{
            await this.web3Contract.methods.updateValidator(_targetValidator,_dockerSha,_dockerName,_chainSpecSha,_chainSpecUrl,_isSigning)
                .estimateGas({
                    from : transactionParams.from, 
                    gas: transactionParams.gas,
                    value: transactionParams.value
                    })
        } catch(ex){
            if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
            const errorResult = await getErrorMessage(this.web3, transactionParams)
            throw new Error(errorResult);
        }

        if(transactionParams.privateKey) {
            return handleRawTx(this.web3, transactionParams)
        }
        else {
            return handleTx(this.web3, transactionParams)
        }
        	}


	async setOwner(_newOwner:string, txParams?:SpecialTx): Promise<TransactionReceipt> {

        const transactionParams: SpecialTx = {}

        transactionParams.gas = (txParams && txParams.gas)? txParams.gas : 0 

        transactionParams.to = this.web3Contract._address

        const txData = await this.web3Contract.methods.setOwner(_newOwner)
            .encodeABI()

        transactionParams.data = txData

        if(!txParams) txParams = {}

        if(txParams.from) {
            transactionParams.from = txParams.from
        }

        if(txParams.value) {
            transactionParams.value = txParams.value
        }

        if(txParams.nonce){
            transactionParams.nonce = txParams.nonce
        }

        if(txParams.privateKey){

            const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
            const senderAddress =  this.web3.eth.accounts.privateKeyToAccount(pk).address
            transactionParams.privateKey = pk

            if(txParams.from) {
                if(txParams.from != senderAddress) throw new Error ('provided PrivateKey and account do not match')
            }
            transactionParams.from = senderAddress
        }

        if(!transactionParams.from) {
            const web3accounts =  await this.web3.eth.getAccounts()
            if(web3accounts.length == 0)throw new Error ("no ethereum account available")

            const web3acc = web3accounts[0]
            if(!web3acc && !transactionParams.privateKey) throw new Error ("no ethereum account available")

            transactionParams.from = web3acc
        }

        if(transactionParams.gas == 0){

            try {
                let gas = await this.web3Contract.methods.setOwner(_newOwner)
                    .estimateGas({ 
                        from: transactionParams.from,
                        value: transactionParams.value
                    })

                 gas = Math.round(gas*2)
                transactionParams.gas = gas 
            } catch(ex) {
                if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
                const errorResult = await getErrorMessage(this.web3, transactionParams)
                throw new Error(errorResult);
            }
        }

        try{
            await this.web3Contract.methods.setOwner(_newOwner)
                .estimateGas({
                    from : transactionParams.from, 
                    gas: transactionParams.gas,
                    value: transactionParams.value
                    })
        } catch(ex){
            if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
            const errorResult = await getErrorMessage(this.web3, transactionParams)
            throw new Error(errorResult);
        }

        if(transactionParams.privateKey) {
            return handleRawTx(this.web3, transactionParams)
        }
        else {
            return handleTx(this.web3, transactionParams)
        }
        	}


	async confirmUpdate(txParams?: SpecialTx): Promise<TransactionReceipt> {

        const transactionParams: SpecialTx = {}

        transactionParams.gas = (txParams && txParams.gas)? txParams.gas : 0 

        transactionParams.to = this.web3Contract._address

        const txData = await this.web3Contract.methods.confirmUpdate()
            .encodeABI()

        transactionParams.data = txData

        if(!txParams) txParams = {}

        if(txParams.from) {
            transactionParams.from = txParams.from
        }

        if(txParams.value) {
            transactionParams.value = txParams.value
        }

        if(txParams.nonce){
            transactionParams.nonce = txParams.nonce
        }

        if(txParams.privateKey){

            const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
            const senderAddress =  this.web3.eth.accounts.privateKeyToAccount(pk).address
            transactionParams.privateKey = pk

            if(txParams.from) {
                if(txParams.from != senderAddress) throw new Error ('provided PrivateKey and account do not match')
            }
            transactionParams.from = senderAddress
        }

        if(!transactionParams.from) {
            const web3accounts =  await this.web3.eth.getAccounts()
            if(web3accounts.length == 0)throw new Error ("no ethereum account available")

            const web3acc = web3accounts[0]
            if(!web3acc && !transactionParams.privateKey) throw new Error ("no ethereum account available")

            transactionParams.from = web3acc
        }

        if(transactionParams.gas == 0){

            try {
                let gas = await this.web3Contract.methods.confirmUpdate()
                    .estimateGas({ 
                        from: transactionParams.from,
                        value: transactionParams.value
                    })

                 gas = Math.round(gas*2)
                transactionParams.gas = gas 
            } catch(ex) {
                if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
                const errorResult = await getErrorMessage(this.web3, transactionParams)
                throw new Error(errorResult);
            }
        }

        try{
            await this.web3Contract.methods.confirmUpdate()
                .estimateGas({
                    from : transactionParams.from, 
                    gas: transactionParams.gas,
                    value: transactionParams.value
                    })
        } catch(ex){
            if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
            const errorResult = await getErrorMessage(this.web3, transactionParams)
            throw new Error(errorResult);
        }

        if(transactionParams.privateKey) {
            return handleRawTx(this.web3, transactionParams)
        }
        else {
            return handleTx(this.web3, transactionParams)
        }
        	}


	async currentState(param0:string, txParams?:SpecialTx): Promise<{0:string ,1:string ,2:string ,3:string ,4:boolean ,5:number ,6:number ,dockerSha:string ,dockerName:string ,chainSpecSha:string ,chainSpecUrl:string ,isSigning:boolean ,updateIntroduced:number ,updateConfirmed:number }> 
            {

                try {
                    const returnValue = (await this.web3Contract.methods.currentState(param0).call(txParams))
                    return returnValue
                    } catch (ex) {
        
                        console.log("call fail")
        
                        const transactionParams: SpecialTx = {}
        
                        transactionParams.gas = (txParams && txParams.gas)? txParams.gas : 0 
                
                        transactionParams.to = this.web3Contract._address
                
                        const txData = await this.web3Contract.methods.currentState(param0)
                            .encodeABI()
                
                        transactionParams.data = txData
                
                        if(!txParams) txParams = {}
                
                        if(txParams.from) {
                            transactionParams.from = txParams.from
                        }
                
                        if(txParams.value) {
                            transactionParams.value = txParams.value
                        }
                
                        if(txParams.nonce){
                            transactionParams.nonce = txParams.nonce
                        }
                
                        if(txParams.privateKey){
                
                            const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
                            const senderAddress =  this.web3.eth.accounts.privateKeyToAccount(pk).address
                            transactionParams.privateKey = pk
                
                            if(txParams.from) {
                                if(txParams.from != senderAddress) throw new Error ('provided PrivateKey and account do not match')
                            }
                            transactionParams.from = senderAddress
                        }
                
                        if(!transactionParams.from) {
                            let web3acc =  (await this.web3.eth.getAccounts())[0]
                            if(!web3acc && !transactionParams.privateKey) { 
                                web3acc = '0x1000000000000000000000000000000000000005'
                            }
                            transactionParams.from = web3acc
                        }
        
                        if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
                        const errorResult = await getErrorMessage(this.web3, transactionParams)
                        throw new Error(errorResult);
                    }
               
                	}


	async RetrieveUpdate(_targetValidator:string, txParams?:SpecialTx): Promise<any> {
 

            try {
            const returnValue = (await this.web3Contract.methods.RetrieveUpdate(_targetValidator).call(txParams))
            return returnValue
            } catch (ex) {

                console.log("call fail")

                const transactionParams: SpecialTx = {}

                transactionParams.gas = (txParams && txParams.gas)? txParams.gas : 0 
        
                transactionParams.to = this.web3Contract._address

                const txData = await this.web3Contract.methods.RetrieveUpdate(_targetValidator)
                    .encodeABI()
                
                transactionParams.data = txData
        
                if(!txParams) txParams = {}
        
                if(txParams.from) {
                    transactionParams.from = txParams.from
                }
        
                if(txParams.value) {
                    transactionParams.value = txParams.value
                }
        
                if(txParams.nonce){
                    transactionParams.nonce = txParams.nonce
                }

                if(txParams.privateKey){
        
                    const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
                    console.log("found pk " + pk)


                    const senderAddress =  this.web3.eth.accounts.privateKeyToAccount(pk).address
                    //transactionParams.privateKey = pk
        
                    console.log("senderAddress " + senderAddress)

                    if(txParams.from) {
                        if(txParams.from != senderAddress) throw new Error ('provided PrivateKey and account do not match')
                    }
                    transactionParams.from = senderAddress
                }
                console.log("call after PK")

                if(!transactionParams.from) {
                    let web3acc =  (await this.web3.eth.getAccounts())[0]
                    if(!web3acc && !transactionParams.privateKey) { 
                        web3acc = '0x1000000000000000000000000000000000000005'
                    }
                    transactionParams.from = web3acc
                }

                if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
                const errorResult = await getErrorMessage(this.web3, transactionParams)
                throw new Error(errorResult);
            }
        
        	}


	async owner(txParams?: SpecialTx): Promise<string> {
 

            try {
            const returnValue = (await this.web3Contract.methods.owner().call(txParams))
            return returnValue
            } catch (ex) {

                console.log("call fail")

                const transactionParams: SpecialTx = {}

                transactionParams.gas = (txParams && txParams.gas)? txParams.gas : 0 
        
                transactionParams.to = this.web3Contract._address

                const txData = await this.web3Contract.methods.owner()
                    .encodeABI()
                
                transactionParams.data = txData
        
                if(!txParams) txParams = {}
        
                if(txParams.from) {
                    transactionParams.from = txParams.from
                }
        
                if(txParams.value) {
                    transactionParams.value = txParams.value
                }
        
                if(txParams.nonce){
                    transactionParams.nonce = txParams.nonce
                }

                if(txParams.privateKey){
        
                    const pk = txParams.privateKey.startsWith('0x') ? txParams.privateKey : '0x' + txParams.privateKey   
                    console.log("found pk " + pk)


                    const senderAddress =  this.web3.eth.accounts.privateKeyToAccount(pk).address
                    //transactionParams.privateKey = pk
        
                    console.log("senderAddress " + senderAddress)

                    if(txParams.from) {
                        if(txParams.from != senderAddress) throw new Error ('provided PrivateKey and account do not match')
                    }
                    transactionParams.from = senderAddress
                }
                console.log("call after PK")

                if(!transactionParams.from) {
                    let web3acc =  (await this.web3.eth.getAccounts())[0]
                    if(!web3acc && !transactionParams.privateKey) { 
                        web3acc = '0x1000000000000000000000000000000000000005'
                    }
                    transactionParams.from = web3acc
                }

                if (!(await getClientVersion(this.web3)).includes('Parity')) throw new Error(ex) ;
                const errorResult = await getErrorMessage(this.web3, transactionParams)
                throw new Error(errorResult);
            }
        
        	}


    }
    