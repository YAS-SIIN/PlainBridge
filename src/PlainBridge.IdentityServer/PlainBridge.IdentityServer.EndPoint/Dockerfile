# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PlainBridge.IdentityServer/PlainBridge.IdentityServer.EndPoint/PlainBridge.IdentityServer.EndPoint.csproj", "src/PlainBridge.IdentityServer/PlainBridge.IdentityServer.EndPoint/"]
COPY ["src/PlainBridge.ServiceDefaults/PlainBridge.ServiceDefaults.csproj", "src/PlainBridge.ServiceDefaults/"]
COPY ["src/PlainBridge.Api/PlainBridge.Api.Application/PlainBridge.Api.Application.csproj", "src/PlainBridge.Api/PlainBridge.Api.Application/"]
COPY ["src/PlainBridge.SharedApplication/PlainBridge.SharedApplication.csproj", "src/PlainBridge.SharedApplication/"]
COPY ["src/PlainBridge.Api/PlainBridge.Api.Infrastructure/PlainBridge.Api.Infrastructure.csproj", "src/PlainBridge.Api/PlainBridge.Api.Infrastructure/"]
COPY ["src/PlainBridge.Api/PlainBridge.Api.Domain/PlainBridge.Api.Domain.csproj", "src/PlainBridge.Api/PlainBridge.Api.Domain/"]
COPY ["src/PlainBridge.Server/PlainBridge.Server.Application/PlainBridge.Server.Application.csproj", "src/PlainBridge.Server/PlainBridge.Server.Application/"]
RUN dotnet restore "./src/PlainBridge.IdentityServer/PlainBridge.IdentityServer.EndPoint/PlainBridge.IdentityServer.EndPoint.csproj"
COPY . .
WORKDIR "/src/src/PlainBridge.IdentityServer/PlainBridge.IdentityServer.EndPoint"
RUN dotnet build "./PlainBridge.IdentityServer.EndPoint.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PlainBridge.IdentityServer.EndPoint.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlainBridge.IdentityServer.EndPoint.dll"]