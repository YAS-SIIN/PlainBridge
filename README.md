# PlainBridge

Expose your local applications to the world — safely.

PlainBridge is a microservice-based end‑to‑end sample/product that demonstrates how to build a modern, cloud‑ready tunneling platform with .NET Aspire. It packages an identity service, backend APIs, a public routing server, a client agent, and an Angular web app into a single distributed application you can run locally with one command.

Why this repository is worth your time
- Built with .NET 9 and .NET Aspire 9.x (new, opinionated distributed app model in the .NET ecosystem)
- Shows best‑practice architecture for modular .NET solutions: clean layering, shared building blocks, and test projects
- Real integrations: Redis (HybridCache), RabbitMQ, Serilog, OpenAPI, Duende BFF + Duende IdentityServer, optional Elasticsearch/OpenTelemetry
- Orchestrated developer experience: Aspire AppHost boots all projects + infra and gives you a dashboard
- Great to learn modern .NET: minimal hosting, configuration binding, DI, background services, messaging patterns, and WebSocket proxying


## What is PlainBridge?
PlainBridge makes your local apps reachable from the internet using a central “Server” that relays HTTP and WebSocket traffic to a “Client” running on your machine. Authentication and authorization are handled by a Duende IdentityServer instance. You manage projects, domains, and clients via a Web UI and a backend API.

High‑level components
- PlainBridge IDS: Duende IdentityServer for secure sign‑in and API protection
- PlainBridge API: management APIs for customers/projects/domains (+ BFF endpoints)
- PlainBridge Web: Angular front‑end for customers to create and configure projects
- PlainBridge Server: public entry point that routes HTTP/WebSocket traffic to the right client
- PlainBridge Client: agent on the user machine that forwards traffic to local apps and returns responses


## Repository layout

```text path=null start=null
PlainBridge/
├─ src/
│  ├─ PlainBridge.AppHost/                # .NET Aspire AppHost (orchestrates everything)
│  ├─ PlainBridge.ServiceDefaults/        # Shared packages, logging, OTEL, service discovery, etc.
│  ├─ PlainBridge.Api/
│  │  ├─ PlainBridge.Api.Domain/          # Domain types
│  │  ├─ PlainBridge.Api.Application/     # App services, handlers
│  │  ├─ PlainBridge.Api.Infrastructure/  # Persistence/integration (Redis, RabbitMQ, etc.)
│  │  └─ PlainBridge.Api.ApiEndPoint/     # ASP.NET Core endpoint (BFF + APIs)
│  ├─ PlainBridge.Server/
│  │  ├─ PlainBridge.Server.Application/  # Routing, buses, WebSocket, cache
│  │  └─ PlainBridge.Server.ApiEndPoint/  # Public-facing ASP.NET Core endpoint
│  ├─ PlainBridge.Client/
│  │  ├─ PlainBridge.Client.Application/  # Client agent app services
│  │  └─ PlainBridge.Client.ApiEndPoint/  # Client endpoint (BFF, login flow)
│  ├─ PlainBridge.IdentityServer/
│  │  └─ PlainBridge.IdentityServer.EndPoint/ # Duende IdentityServer
│  ├─ PlainBridge.SharedApplication/      # Shared mediator, DTOs, abstractions
│  ├─ PlainBridge.SharedDomain/           # Shared domain primitives
│  └─ PlainBridge.Web/
│     └─ PlainBridge.Web.UI/              # Angular front-end (esproj)
├─ infra/
│  ├─ docker-compose.yaml                 # Compose env that AppHost can target
│  └─ .env                                # Ports and image names for compose
└─ tests/                                 # API/Server/Client test projects
```


## Architecture in detail (Aspire best practices)
This solution uses .NET Aspire to define and wire together multiple processes, infrastructure dependencies, and a developer dashboard.

Orchestration
- AppHost: src/PlainBridge.AppHost/Program.cs uses DistributedApplication.CreateBuilder and adds:
  - Redis with RedisInsight, RabbitMQ with management UI, optional Elasticsearch (non‑Dev)
  - Projects: identityserver-endpoint, api-endpoint, server-endpoint, client-endpoint
  - Npm app: angular-webui (Angular served during dev)
  - Health/ordering: resources are referenced and awaited with WaitFor to ensure correct startup order
  - Compute environment: builder.AddDockerComposeEnvironment("plain-bridge") maps to infra/docker-compose.yaml for local dependencies

Core patterns and libraries
- Authentication/Authorization
  - Duende IdentityServer with code flow clients for API (BFF) and Client app
  - Duende BFF + YARP in API and Client endpoints (cookie + OIDC, token forwarding, remote APIs)
- Caching and performance
  - Microsoft HybridCache backed by Redis; default and local expirations are configured via ApplicationSettings
- Messaging and coordination
  - RabbitMQ used for control-plane messages between Server and Client (e.g., “server_bus”, “client_bus”), and for HTTP/WebSocket packet routing queues
- WebSocket proxying
  - Server listens for WebSocket frames, republishes via RabbitMQ; Client reads from queues and writes back to browser
- Observability and logging
  - Serilog console sink for development; optional Elasticsearch sink in non‑Dev; OpenAPI in Dev; Aspire Dashboard + OTEL wiring in compose
- Clean layering and shared building blocks
  - API split into Domain/Application/Infrastructure/Endpoint
  - SharedApplication contains a lightweight Mediator implementation and cross‑cutting abstractions

Selected flows
- Sign-in: Users authenticate against IdentityServer. API/Client use BFF with OpenID Connect for session and token handling.
- Expose app: A user registers a project/domain via Web UI. Server and Client coordinate over RabbitMQ to establish routes, and traffic is proxied over HTTP or WebSockets.


## Prerequisites
- .NET SDK 9.0+
- Node.js 20+ (for the Angular UI during development)
- Docker Desktop (for Redis, RabbitMQ, optional Elasticsearch, Aspire dashboard)
- Windows/Mac/Linux; dev certs trusted locally

Trust a dev certificate (once on your machine):
```powershell path=null start=null
dotnet dev-certs https --trust
```


## Getting started (recommended: .NET Aspire)
Run everything with a single command using AppHost:
```powershell path=null start=null
# from the repository root
dotnet run --project src/PlainBridge.AppHost/PlainBridge.AppHost.csproj
```
What you’ll get
- Aspire Dashboard (resources, logs, traces)
- Redis + RedisInsight, RabbitMQ + Management UI
- IdentityServer, API, Server, Client, and Angular Web UI

Default ports (configurable in src/PlainBridge.AppHost/appsettings.json)
- API:        https://localhost:5001
- Server:     https://localhost:5002
- Identity:   https://localhost:5003
- Web (UI):   http://localhost:5004
- Client:     https://localhost:5005

Sign in with the Duende IdentityServer UI at https://localhost:5003 and then use the Angular UI at http://localhost:5004.


### Alternative: run only the infra via Docker Compose
If you want only the infra containers (Redis, RabbitMQ, Elasticsearch, dashboard):
```powershell path=null start=null
# from the repository root
docker compose --env-file infra/.env -f infra/docker-compose.yaml up -d cache messaging elasticsearch plain-bridge-dashboard
```
Then launch the .NET projects from your IDE or with dotnet run.


## Configuration
Each service binds ApplicationSettings from appsettings.json (and environment variables). Examples:
- AppHost port selection: src/PlainBridge.AppHost/appsettings.json
- API settings: src/PlainBridge.Api/PlainBridge.Api.ApiEndPoint/appsettings.Development.json
- Server settings: src/PlainBridge.Server/PlainBridge.Server.ApiEndPoint/appsettings.json
- Client settings: src/PlainBridge.Client/PlainBridge.Client.ApiEndPoint/appsettings.json
- IdentityServer client redirect URIs are set in src/PlainBridge.IdentityServer/PlainBridge.IdentityServer.EndPoint/Config.cs

For compose deployments, see infra/.env for port and image mappings.


## Screenshots

![Aspire Graph](https://github.com/YAS-SIIN/PlainBridge/blob/master/docs/images/resources-graph.png)
Small, connected graph showing Redis, RabbitMQ, and the API/Server/Client/IDS/Angular apps.



![Aspire Table](https://github.com/YAS-SIIN/PlainBridge/blob/master/docs/images/resources-table.png)
Tabular view with URLs for each running resource.



![Identity Login](https://github.com/YAS-SIIN/PlainBridge/blob/master/docs/images/ids-login.png)
Duende IdentityServer login page (local user and Google button placeholder).



![Web Dashboard](https://github.com/YAS-SIIN/PlainBridge/blob/master/docs/images/web-dashboard.png)
Angular UI landing dashboard after sign-in.
 


## Development tips
- OpenAPI UIs are enabled in Development for API/Client endpoints
- Serilog console templates make logs compact; Elasticsearch sink is enabled outside Development
- The custom Mediator in SharedApplication is a good read for understanding request/handler pipelines


## Security and production notes
- Configuration and secrets in this repo are for development only
- Replace Google OAuth placeholders in IdentityServer before any real use
- Review CORS, cookie policies, and TLS termination for production


## Contributing
Issues and PRs are welcome. If you plan a larger change, please open an issue first to discuss the approach.


## License
This project is licensed under the MIT License — see [LICENSE](LICENSE) for details.
