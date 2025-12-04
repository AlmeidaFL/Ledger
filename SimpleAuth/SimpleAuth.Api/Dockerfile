FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ServiceCommons/ServiceCommons/ServiceCommons.csproj ServiceCommons/ServiceCommons/
COPY ServiceCommons/ServiceCommons.ApiKey/ServiceCommons.ApiKey.csproj ServiceCommons/ServiceCommons.ApiKey/
COPY ServiceCommons/ServiceCommons.OpenTelemetry/ServiceCommons.OpenTelemetry.csproj ServiceCommons/ServiceCommons.OpenTelemetry/

COPY SimpleAuth/SimpleAuth.Api/SimpleAuth.Api.csproj SimpleAuth/SimpleAuth.Api/
COPY SimpleAuth/SimpleAuth.MigrationsRunner/SimpleAuth.MigrationsRunner.csproj SimpleAuth/SimpleAuth.MigrationsRunner/

RUN dotnet restore "SimpleAuth/SimpleAuth.Api/SimpleAuth.Api.csproj"
RUN dotnet restore "SimpleAuth/SimpleAuth.MigrationsRunner/SimpleAuth.MigrationsRunner.csproj"

COPY ServiceCommons/ ServiceCommons/
COPY SimpleAuth/ SimpleAuth/

WORKDIR /src/SimpleAuth/SimpleAuth.Api
RUN dotnet build "SimpleAuth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

WORKDIR /src/SimpleAuth/SimpleAuth.MigrationsRunner
RUN dotnet build "SimpleAuth.MigrationsRunner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "/src/SimpleAuth/SimpleAuth.Api/SimpleAuth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "/src/SimpleAuth/SimpleAuth.MigrationsRunner/SimpleAuth.MigrationsRunner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleAuth.Api.dll"]
