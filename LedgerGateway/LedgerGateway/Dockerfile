FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

RUN dotnet tool install --global NSwag.ConsoleCore --version 14.6.3
ENV PATH="$PATH:/root/.dotnet/tools"

COPY open-api/ ./open-api/
COPY ServiceCommons/ServiceCommons/ServiceCommons.csproj ServiceCommons/ServiceCommons/
COPY ServiceCommons/ServiceCommons.OpenTelemetry/ServiceCommons.OpenTelemetry.csproj ServiceCommons/ServiceCommons.OpenTelemetry/
COPY ServiceCommons/ServiceCommons.ApiKey/ServiceCommons.ApiKey.csproj ServiceCommons/ServiceCommons.ApiKey/
COPY LedgerGateway/LedgerGateway/LedgerGateway.csproj LedgerGateway/LedgerGateway/

RUN dotnet restore "LedgerGateway/LedgerGateway/LedgerGateway.csproj"

COPY ServiceCommons/ ServiceCommons/
COPY LedgerGateway/ LedgerGateway/

WORKDIR /src/LedgerGateway/LedgerGateway
RUN echo "=== DEBUG STRUCTURE ===" && pwd && ls -R /src
RUN dotnet build "LedgerGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "LedgerGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LedgerGateway.dll"]