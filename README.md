# Node Control

Component to update parity and chain spec via on-chain events

## Dependecies

- Current dotnet SDK (`v2.2`)
- `Docker.DotNet` -> interaction with the docker daemon
- `Nethereum` -> interaction with the smart contracts


## Docker build

The GitLab CI will build the application, run the tests and then publish the resulting docker image.

To run an equvalent build locally:

```
docker build -t nc-local -f Local.Dockerfile .
```

This will first prepare a build environment with node.js and dotnet core to be able to run the C# compillation as well as the solidity contract preparation and deployment to a ganache instance on the container.

**NOTE: This repository is not authoritive for the NodeControl contract. It only holds a copy to facillitate the unit tests**

The process will also run the tests and produce a runnable image of nodecontrol (as `nc-local`)

## Configuration

Nodecontrol can be configured using environment vairables:

- `CONTRACT_ADDRESS` - Address of the lookup contract. The address of the "real" nodecontrol contract is retrieved from there during startup
- `STACK_PATH` - Path to the `docker-stack` directory on the host. name inside container has to match outside. See example compose section.
- `RPC_ENDPOINT` - JSON-RPC http endpoint. Preferably the local parity.
- `VALIDATOR_ADDRESS` - Public address of the validator nodecontrol runs on

## Example docker-compose section

This is an example configuration of node-control, taken from the `docker-compose.yml` of a validator node.

```
nodecontrol:                                        
    image: energyweb/nodecontrol:${NODECONTROL_VERSION}
    restart: always
    volumes:      
      - $PWD:$PWD                                     
      - /var/run/docker.sock:/var/run/docker.sock
    environment:
      - CONTRACT_ADDRESS=0x1204700000000000000000000000000000000007
      - STACK_PATH=$PWD
      - RPC_ENDPOINT=http://parity:8545
      - VALIDATOR_ADDRESS=${VALIDATOR_ADDRESS}
```

## Build

- Have dotnet SDK installed
- `dotnet build`

## Tests

- Have the build pass
- switch into the tests directory `cd tests`
- run the tests `dotnet test`

To obtain coverage and test report: `dotnet test /p:CollectCoverage=true /p:Include='[src*]*' /p:CoverletOutputFormat=\"opencover,lcov\" /p:CoverletOutput=../lcov --logger "trx;LogFileName=TestResults.trx"`

Remark: Per default the `ContractWrapperTests` are skipped as they are not self-contained. They need a preparewd chain with the contracts intialized.
