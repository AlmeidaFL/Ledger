FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ServiceCommons/ServiceCommons/ServiceCommons.csproj ServiceCommons/ServiceCommons/
COPY EventRelayWorker/EventRelayWorker.csproj EventRelayWorker/

RUN dotnet restore "EventRelayWorker/EventRelayWorker.csproj"

COPY ServiceCommons/ ServiceCommons/
COPY EventRelayWorker/ EventRelayWorker/

WORKDIR /src/EventRelayWorker
RUN dotnet build "EventRelayWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "/src/EventRelayWorker/EventRelayWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventRelayWorker.dll"]
