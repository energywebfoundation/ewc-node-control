# Node Control

Component to update parity and chain spec via on-chain events

## Dependecies

- Current dotnet SDK (`v2.2`)
- `Docker.DotNet` -> interaction with the docker daemon
- `Nethereum` -> interaction with the smart contracts

## Build

- Have dotnet SDK installed
- `dotnet build`

## Tests

- Have the build pass
- switch into the tests directory `cd tests`
- run the tests `dotnet test`

To obtain coverage and test report: `dotnet test /p:CollectCoverage=true /p:Include='[src*]*' /p:CoverletOutputFormat=\"opencover,lcov\" /p:CoverletOutput=../lcov --logger "trx;LogFileName=TestResults.trx"`

Remark: Per default the `ContractWrapperTests` are skipped as they are not self-contained. They need a preparewd chain with the contracts intialized.
