FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ServiceCommons/ServiceCommons/ServiceCommons.csproj ServiceCommons/ServiceCommons/
COPY ServiceCommons/ServiceCommons.ApiKey/ServiceCommons.ApiKey.csproj ServiceCommons/ServiceCommons.ApiKey/
COPY ServiceCommons/ServiceCommons.OpenTelemetry/ServiceCommons.OpenTelemetry.csproj ServiceCommons/ServiceCommons.OpenTelemetry/

COPY UserApi/UserApi/UserApi.csproj UserApi/UserApi/
COPY UserApi/UserApi.MigrationsRunner/UserApi.MigrationsRunner.csproj UserApi/UserApi.MigrationsRunner/

RUN dotnet restore "UserApi/UserApi/UserApi.csproj"
RUN dotnet restore "UserApi/UserApi.MigrationsRunner/UserApi.MigrationsRunner.csproj"

COPY ServiceCommons/ ServiceCommons/
COPY UserApi/ UserApi/

WORKDIR /src/UserApi/UserApi
RUN dotnet build "UserApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

WORKDIR /src/UserApi/UserApi.MigrationsRunner
RUN dotnet build "UserApi.MigrationsRunner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "/src/UserApi/UserApi/UserApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
RUN dotnet publish "/src/UserApi/UserApi.MigrationsRunner/UserApi.MigrationsRunner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserApi.dll"]