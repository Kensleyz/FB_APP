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
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "PageBoostAI.Api.dll"]
