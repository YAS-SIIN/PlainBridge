# PlainBridge Developer & Agent Guide (CLAUDE.md)

## 1. Project Purpose & Architecture
PlainBridge is a distributed reverse-tunneling platform built on .NET 10 and Aspire. It exposes local HTTP, WebSocket, and raw TCP services to public internet domains via a centralized routing server, messaging bus, and client agents.

### Core Services & Responsibilities
- `PlainBridge.AppHost`: Local orchestration via .NET Aspire (`AppHost.cs`), provisioning Redis, RabbitMQ, and services.
- `PlainBridge.IdentityServer`: Duende IdentityServer (OIDC/OAuth2) with in-memory ASP.NET Core Identity.
- `PlainBridge.Api`: Control plane managing `HostApplication`, `ServerApplication`, and `User` entities; provides BFF and token exchange endpoints.
- `PlainBridge.Server`: Public entry gateway; proxies incoming public HTTP/WebSocket traffic to clients via RabbitMQ.
- `PlainBridge.Client`: Local client agent; consumes requests from RabbitMQ, proxies to localhost targets, and returns responses.
- `PlainBridge.Shared`: Domain primitives (`BaseAggregate`), shared DTOs (`HttpRequestDto`, `HttpResponseDto`), and custom `IMediator`.
- `PlainBridge.Web`: Angular 22 SPA dashboard for application, domain, and user profile management.

## 2. Tech Stack & Architectural Patterns
- **Backend:** .NET 10, C# 13/14, ASP.NET Core Minimal APIs, Duende IdentityServer 7.x, Duende BFF.
- **Messaging:** RabbitMQ 4.1 via `RabbitMQ.Client` 7.x async APIs (direct exchanges and dedicated queues).
- **Caching:** Redis 7.4 via `Microsoft.Extensions.Caching.Hybrid` (HybridCache L1 memory + L2 Redis).
- **Frontend:** Angular 22, RxJS 7.8, Angular Material, TypeScript 5.7+ (`PlainBridge.Web.UI`).
- **Patterns:** Clean Architecture in API (Domain -> Application -> Infrastructure -> ApiEndPoint), CQRS via custom reflection Mediator, BFF (Backend for Frontend), Request-Reply correlation over RabbitMQ via `TaskCompletionSource`.

## 3. Key Project Paths & Boundaries
- `src/PlainBridge.AppHost/PlainBridge.AppHost.AppHost/AppHost.cs`: Aspire entry point.
- `src/PlainBridge.IdentityServer/src/PlainBridge.IdentityServer.EndPoint/`: IdentityServer configuration (`Config.cs`, `Program.cs`).
- `src/PlainBridge.Api/src/`: Clean architecture layers for API control plane.
- `src/PlainBridge.Server/src/`: Server gateway (`Middleware/HttpRequestProxyMiddleware.cs`, `Services/WebSocket/WebSocketService.cs`).
- `src/PlainBridge.Client/src/`: Client agent (`Handler/HttpRequest/HttpRequestHandler.cs`, `Helpers/Http/HttpHelper.cs`).
- `src/PlainBridge.Shared/src/`: Cross-cutting contracts and Mediator implementation (`Mediator/Mediator.cs`).
- `src/PlainBridge.Web/src/PlainBridge.Web.UI/`: Angular application (`src/app/features/`, `src/app/services/`).
- `infra/`: `docker-compose.yaml` and `.env` for containerized infrastructure.
- `tests/PlainBridge.Tests/`: Cross-service Aspire integration tests.

## 4. Build, Run & Test Commands
```bash
# Build entire .NET solution
dotnet build PlainBridge.slnx

# Run all services + infrastructure via Aspire
dotnet run --project src/PlainBridge.AppHost/PlainBridge.AppHost.AppHost/PlainBridge.AppHost.AppHost.csproj

# Run only infrastructure containers (Redis & RabbitMQ)
docker compose --env-file infra/.env -f infra/docker-compose.yaml up -d cache messaging

# Run .NET Tests
dotnet test tests/PlainBridge.Tests/PlainBridge.Tests.csproj
dotnet test src/PlainBridge.Api/tests/PlainBridge.Api.Tests/PlainBridge.Api.Tests.csproj
dotnet test src/PlainBridge.Server/tests/PlainBridge.Server.Tests/PlainBridge.Server.Tests.csproj
dotnet test src/PlainBridge.Client/tests/PlainBridge.Client.Tests/PlainBridge.Client.Tests.csproj

# Run Angular UI
cd src/PlainBridge.Web/src/PlainBridge.Web.UI
npm install
npm start
```

## 5. Configuration, Ports & Environment
*Note: Discrepancies exist across configuration files. Verify active values before binding.*
- **Aspire AppHost (`appsettings.json`):** API: `5051`, Server: `5052`, Identity: `5053`, Web: `5054`, Client: `5055`.
- **Docker Compose (`infra/.env`):** API: `5001`, Server: `5002`, Identity: `5003`, Web: `5004`, Client: `5005`.
- **Angular (`environment.ts`):** `apiUrl: https://localhost:5051`, `identityServerUrl: https://localhost:5053`.

## 6. Communication Contracts & Messaging Workflows
- **API -> Server Notifications:** Published to exchange `external_bus` with routing key `api_to_server` -> consumed from queue `api_to_server_external_bus`. Payload is a string literal (e.g. `"Host_Application_Created"`).
- **HTTP Tunneling:**
  - Server receives public request -> caches `TaskCompletionSource` with `requestId` -> publishes `HttpRequestDto` to exchange `request`, routing key `{username}` (queue `{username}_request_bus`).
  - Client forwards to local `InternalUrl` -> publishes `HttpResponseDto` to exchange `response` -> consumed from queue `response_bus` -> Server completes `TaskCompletionSource` by `MessageId`.
- **Client Handshake:** Client requests server apps via queue `server_bus`; Server replies to exchange `client_bus` (queue `{username}_client_bus`).

## 7. Known Bugs & Implementation Defects (DO NOT WORSEN)
- **Broken TCP Port Tunneling (Confirmed):**
  - Queue mismatch: Server publishes to `{user}client_shared_port_network_packets` (`ServerApplicationConsumerService.cs:78`); Client listens on `{username}_client_shareport_network_packets` (`SharePortSocketService.cs:26`).
  - Response queue mismatch: Server publishes to `{user}_client_user_port_network_packets` (`ServerApplicationConsumerService.cs:135`); Client listens on `{username}_client_useport_network_packets` (`UsePortSocketService.cs:99`).
  - Header mismatches: `"user_port"` vs `"useport_port"`, `"shared_port"` vs `"sharedport_port"`.
  - Infinite busy-spin loop in `UsePortSocketService.cs:64` consumes 100% CPU when disconnected.
- **WebSocket Return Consumer Omitted (Confirmed):** `_webSocketService.InitializeConsumerAsync()` is omitted from Server `Worker.cs:20-22`. Return WebSocket messages are never consumed.
- **Server API Route Missing `/api` (Confirmed):** `PlainBridgeApiClientService.cs:30` requests `${PlainBridgeApiAddress}/HostApplication` instead of `/api/HostApplication`, returning 404.
- **HybridCache Redis Serialization Bug (Confirmed):** `CacheManagement.cs:28` stores live `IWebSocketManagement` instances in `HybridCache`, which fails or corrupts on Redis L2 serialization.
- **Single Shared Response Queue (Confirmed):** Server instances share queue `response_bus`, preventing horizontal multi-instance scaling.
- **Resource Leaks (Confirmed):** RabbitMQ channels are created without disposal in `RabbitMQEventBus.cs:14` and `HttpRequestProxyMiddleware.cs:97`. `ResponseCompletionSourcesManagement` entries leak on timeout.
- **Non-Thread-Safe Dictionary (Confirmed):** `HttpRequestProxyMiddleware.cs:15` uses non-concurrent `Dictionary<string, bool> _initializedQueues` in singleton middleware.

## 8. Security & Hygiene Hazards (DO NOT WORSEN)
- **Hardcoded Secrets:** Custom JWT key hardcoded in `TokenService.cs:35`; OAuth secret hardcoded in `Config.cs`.
- **BFF Anti-Pattern:** `LoginEndpoint.cs:75` passes tokens via URL query string `?access_token={tokenp}`; logs cleartext tokens (`LoginEndpoint.cs:41`); Client reads cleartext file `profile` without auth validation (`LoginEndpoint.cs:30`).
- **Disabled TLS Validation:** `HttpHelper.cs:59` uses `ServerCertificateCustomValidationCallback = (_, _, _, _) => true`.
- **In-Memory Volatility:** Both IdentityServer and API use EF Core `UseInMemoryDatabase`.
- **Missing Authorization / IDOR:** Host and Server application endpoints lack user ownership validation; IdentityServer `UserEndpoint.cs` allows unauthenticated user and password changes.
- **Committed Artifacts:** Committed session files (`cookies.txt`, `headers.txt`, `profile`, `keys/`) exist in repository. Do not add credentials or rely on committed state.

## 9. Mandatory Instructions for AI Coding Agents
1. **Inspect Code First:** Treat actual source code as authoritative over README or documentation. Verify whether a bug exists before touching related files.
2. **Follow Existing Patterns:** Respect Clean Architecture layers in `PlainBridge.Api`, use `IMediator` for CQRS, and follow existing DTO conventions.
3. **Never Compromise Security:** Do not introduce hardcoded secrets, bypass TLS validation, expose tokens in query strings or logs, or bypass authorization.
4. **Avoid Channel & Memory Leaks:** Always dispose RabbitMQ channels (`await using`) and clean up `TaskCompletionSource` dictionaries.
5. **Scope Changes Carefully:** Do not make unrelated changes, redesign architectures unilaterally, or modify untouched services.
6. **Validate Changes:** Run project-specific builds and unit tests after making changes (`dotnet build`, `dotnet test`).

