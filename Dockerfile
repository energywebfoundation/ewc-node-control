FROM microsoft/dotnet:2.2-aspnetcore-runtime
RUN curl -L "https://github.com/docker/compose/releases/download/1.24.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/bin/docker-compose
RUN chmod +x /usr/bin/docker-compose

WORKDIR /app
ADD src/build .
ENTRYPOINT ["dotnet", "src.dll"]