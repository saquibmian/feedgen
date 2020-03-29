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

EXPOSE 80/tcp
VOLUME /feeds
ENV FEEDGEN__ROOTDIRECTORY /feeds
ENV FEEDGEN__EXTERNALADDRESS http://localhost:80

ENTRYPOINT ["dotnet", "FeedGen.Server.dll"]
