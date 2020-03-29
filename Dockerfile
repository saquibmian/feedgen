FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
WORKDIR /app

COPY FeedGen.sln ./
COPY FeedGen.Server/FeedGen.Server.csproj ./FeedGen.Server/
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

# === Build runtime image==
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=builder /app/out .
ENTRYPOINT ["dotnet", "FeedGen.Server.dll"]