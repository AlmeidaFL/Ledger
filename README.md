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

## Run with Docker

### Prerequisites 

#### Required
- **Docker** version 20.10 or higher
- **Docker Compose** version 2.0 or higher (included in Docker Desktop)

#### Optional
- **[Signoz](https://github.com/SigNoz/signoz/tree/main/deploy)** for observability and telemetry collection

### Running the Application

#### Basic Setup (All Services)

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

### Optional: Observability Setup with Signoz

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

## Run with Kubernets (k8s) locally

### Prerequisites 

#### Required
- **Docker Desktop** (includes Docker, Docker Compose and Kubernetes)
- **Bash interpreter**
- **Helm** version 4.0.4 >=

#### Optional
- **[Signoz with Helm](https://signoz.io/docs/install/kubernetes/local/)** for observability and telemetry collection. Follow instructions or see steps bellow

#### Images

1. Build images with either `docker build -f <Dockerfile>` on all services or build them with `docker compose up --build`
2. Also, since this project allow you to also deploy to AWS, it may be necessary to adjust k8s/infra.yaml files on each service to pull images locally. 

### Running the Application

#### Basic Setup (All Services)

1. From the project root directory, run:
```bash
./k8s/startup.sh
```
This script applies all Kubernetes manifests (databases, brokers, services and apps) in the correct order.

This will start all core services:
- PostgreSQL database
- Simple Auth API
- User API
- Financial Service
- Event Relay Worker
- Apache Kafka broker
- Kafka UI
- Front-end application
- Ledger Gateway


#### Tear down basic setup

1. From the project root directory, run:
```bash
./k8s/teardown.sh
```

### Optional: Observability Setup with Signoz

To enable distributed tracing and metrics collection:

1. Run the following instructions from your terminal
```
helm repo add signoz https://charts.signoz.io
helm repo update
helm pull signoz/signoz --untar
cd signoz
helm install signoz .
```

3. Run `kubectl port-forward -n <namespace> svc/signoz 8080:8080`. If not specified in signoz installation, the \<namespace\> will be **default**

4. Start the Ledger services (they are already configured to connect to `http://signoz-otel-collector:4317`)

5. Access the Signoz UI at `http://localhost:8080` to view traces, metrics, and logs