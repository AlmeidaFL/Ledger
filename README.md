# License

Shield: [![CC BY-NC 4.0][cc-by-nc-shield]][cc-by-nc]

This work is licensed under a
[Creative Commons Attribution-NonCommercial 4.0 International License][cc-by-nc].

This project uses third-party libraries under their own respective licenses.
No modification of third-party licenses is intended.

[![CC BY-NC 4.0][cc-by-nc-image]][cc-by-nc]

[cc-by-nc]: https://creativecommons.org/licenses/by-nc/4.0/
[cc-by-nc-image]: https://licensebuttons.net/l/by-nc/4.0/88x31.png
[cc-by-nc-shield]: https://img.shields.io/badge/License-CC%20BY--NC%204.0-lightgrey.svg

# Ledger

## Architecture

![architecture](./documentation/high-overview-archictecure.svg)


# Code under construction

# How to run

## Prerequisites

### Required
- **Docker** version 20.10 or higher
- **Docker Compose** version 2.0 or higher (included in Docker Desktop)

### Optional
- **[Signoz](https://github.com/SigNoz/signoz/tree/main/deploy)** for observability and telemetry collection

## Running the Application

### Basic Setup (All Services)

1. From the project root directory, run:
```bash
docker compose up
```

This will start all core services:
- PostgreSQL database (port 5432)
- Simple Auth API (port 5001)
- User API (port 5002)
- Financial Service (port 5003)
- Event Relay Worker (port 5005)
- Apache Kafka broker (port 29092)
- Kafka UI (port 8081)
- Front-end application (port 5006)
- Ledger Gateway (port 5000)

## Optional: Observability Setup with Signoz

To enable distributed tracing and metrics collection:

1. Create a shared Docker network:
```bash
docker network create signoz-ledger-net
```

2. Configure Signoz to use the shared network:
   - Navigate to your Signoz installation: `signoz/deploy/docker/docker-compose.yaml`
   - Add the network configuration:
```yaml
networks:
  signoz-ledger-net:
    external: true
```
   - Add `signoz-ledger-net` to the **x-common** networks section

3. Start Signoz according to its documentation

4. Start the Ledger services (they are already configured to connect to `http://signoz-otel-collector:4317`)

5. Access the Signoz UI at `http://localhost:8080` to view traces, metrics, and logs