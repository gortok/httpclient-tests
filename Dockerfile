# Pull base image.
FROM microsoft/dotnet:1.1.1-runtime

ARG source=./src/bin/Release/netcoreapp1.1/publish

# Expose ports
EXPOSE 80

WORKDIR /app

COPY $source .

ENTRYPOINT ["dotnet", "HttpClientAsync.dll"]
