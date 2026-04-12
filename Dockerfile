# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY *.sln .
COPY src/PageBoostAI.Domain/*.csproj src/PageBoostAI.Domain/
COPY src/PageBoostAI.Application/*.csproj src/PageBoostAI.Application/
COPY src/PageBoostAI.Infrastructure/*.csproj src/PageBoostAI.Infrastructure/
COPY src/PageBoostAI.Api/*.csproj src/PageBoostAI.Api/
COPY src/PageBoostAI.Tests/*.csproj src/PageBoostAI.Tests/
RUN dotnet restore
COPY . .
RUN dotnet publish src/PageBoostAI.Api -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 10000
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:${PORT:-10000}/health || exit 1
ENTRYPOINT ["/bin/sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet PageBoostAI.Api.dll"]
