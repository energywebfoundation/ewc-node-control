FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app
ADD src/build .
ENTRYPOINT ["dotnet", "src.dll"]