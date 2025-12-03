FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ServiceCommons/ServiceCommons/ServiceCommons.csproj ServiceCommons/ServiceCommons/
COPY ServiceCommons/ServiceCommons.ApiKey/ServiceCommons.ApiKey.csproj ServiceCommons/ServiceCommons.ApiKey/
COPY ServiceCommons/ServiceCommons.OpenTelemetry/ServiceCommons.OpenTelemetry.csproj ServiceCommons/ServiceCommons.OpenTelemetry/
COPY FinancialService/FinancialService/FinancialService.csproj FinancialService/FinancialService/

RUN dotnet restore "FinancialService/FinancialService/FinancialService.csproj"

COPY ServiceCommons/ ServiceCommons/
COPY FinancialService/ FinancialService/

WORKDIR /src/FinancialService/FinancialService
RUN dotnet build "FinancialService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "FinancialService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinancialService.dll"]
