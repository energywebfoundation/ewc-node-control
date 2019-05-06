
export PATH="$PATH:/root/.dotnet/tools"
echo "==== Preparing Ganache and NodeControl Contract ===="
cd contract-prepare
npm install -g typescript
npm install
npm install
npm run start-ganache > /dev/null 2>&1 &
sleep 5
npm run deploy-and-prime

echo "==== Build and test ===="
cd ..
dotnet restore
dotnet build
dotnet test /p:CollectCoverage=true /p:Include='[src*]*' /p:CoverletOutputFormat=\"opencover,lcov\" /p:CoverletOutput=../lcov --logger "trx;LogFileName=TestResults.trx"
trx2junit tests/TestResults/TestResults.trx

echo "==== Publish ====="
dotnet publish -c Release -o build