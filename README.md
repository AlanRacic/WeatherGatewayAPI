# WeatherGatewayAPI

### ASP.NET Core integration API with typed HttpClient, JWT authorization, structured error handling, and Azure deployment

WeatherGatewayAPI is a **.NET 10 / ASP.NET Core Web API** that acts as a gateway between API clients and an external weather service.

The application validates incoming requests, communicates with the external provider through a typed `HttpClient`, maps provider responses into internal DTOs, and exposes a controlled API contract to clients.

The project also demonstrates **JWT Bearer authorization**, centralized exception handling, structured logging, configuration through the Options pattern, and automated deployment to **Azure App Service** with GitHub Actions.

---

## Request Flow

```text
Client
  ↓
JWT Bearer authorization
  ↓
WeatherController
  ↓
IExternalService
  ↓
ExternalService
  ↓
Typed HttpClient
  ↓
External Weather API
  ↓
ExternalWeatherResponse
  ↓
WeatherDto
  ↓
API response
```

The weather endpoints are protected with `[Authorize]`.

Controllers validate incoming city values and delegate external communication to `IExternalService`, keeping HTTP integration logic outside the API controller.

---

## External API Integration

External communication is implemented through `ExternalService` behind the `IExternalService` abstraction.

The typed `HttpClient` is registered through dependency injection and configured with:

* external API base URL
* configurable request timeout
* settings bound through the Options pattern
* asynchronous HTTP communication

City values are URI-encoded before being included in external requests:

```csharp
var encodedCity = Uri.EscapeDataString(city);
```

External `HttpResponseMessage` instances are disposed after each request.

Provider responses are deserialized into an integration-specific model and mapped into the API response DTO:

```text
External Weather API
        ↓
ExternalWeatherResponse
        ↓
WeatherDto
        ↓
Client
```

This prevents the complete external provider payload from becoming part of the public API contract.

---

## API Behavior

For a successful request, the gateway returns a simplified weather response containing:

```json
{
  "city": "City name",
  "temperature": 20.5,
  "description": "Weather description"
}
```

The API distinguishes between client and upstream failures:

* invalid or empty city input → `400 Bad Request`
* city not found by the external provider → `404 Not Found`
* other unsuccessful upstream responses → `502 Bad Gateway`

This keeps external provider failures separate from failures originating inside the gateway itself.

---

## Authentication & Authorization

Weather endpoints use **JWT Bearer authentication** and are protected with:

```csharp
[Authorize]
```

JWT validation includes:

* issuer validation
* audience validation
* token lifetime validation
* signing-key validation

The application intentionally does **not** implement a persistent user store or a complete login system.

For local testing, a short-lived development token can be generated through:

```text
POST /api/auth/token
```

The token endpoint is available only in the **Development** environment.

In Production / Azure App Service, development token generation is not exposed.

---

## Error Handling & Logging

A custom exception-handling middleware centralizes unexpected failures and returns consistent JSON error responses.

The application handles integration-related failures including external HTTP communication errors and request timeouts separately from other unhandled exceptions.

Structured logging with `ILogger` records events such as:

* incoming weather requests
* input validation
* external API calls
* upstream HTTP status codes
* response mapping
* not-found results
* external-service failures

Parameterized log messages preserve structured values such as city names and HTTP status codes.

---

## Configuration & Secrets

Non-sensitive configuration is stored in `appsettings.json`, including values such as:

* external API base URL
* request timeout
* JWT issuer
* JWT audience

Sensitive configuration is kept outside source control:

```text
ExternalWeatherApi:ApiKey
Jwt:Key
```

### Local Development

The project uses **ASP.NET Core User Secrets** for local sensitive values.

In Visual Studio:

```text
Right-click WeatherGatewayAPI
→ Manage User Secrets
```

Example:

```json
{
  "ExternalWeatherApi": {
    "ApiKey": "your-local-api-key"
  },
  "Jwt": {
    "Key": "your-strong-local-signing-key"
  }
}
```

Real API keys and signing keys should never be committed to the repository.

### Azure App Service

Production secrets are supplied through **Azure App Service environment variables / application settings**.

For hierarchical ASP.NET Core configuration, Azure uses double underscores:

```text
ExternalWeatherApi__ApiKey
Jwt__Key
```

This allows committed configuration files to remain free of deployment credentials.

---

## Running Locally

### Prerequisites

* .NET 10 SDK
* valid external weather API key
* configured local JWT signing key

Restore dependencies:

```bash
dotnet restore
```

Run the application:

```bash
dotnet run --project WeatherGatewayAPI/WeatherGatewayAPI.csproj
```

In Development, Swagger / OpenAPI can be used to inspect the API and obtain a development JWT for testing protected endpoints.

---

## CI/CD & Azure Deployment

The application is deployed to **Azure App Service** through GitHub Actions.

```text
Push to master
      ↓
GitHub Actions
      ↓
Restore & Build
      ↓
dotnet publish
      ↓
Deployment artifact
      ↓
OIDC authentication
      ↓
Azure App Service
```

The workflow separates build and deployment responsibilities and:

* restores and builds the .NET application
* publishes the application in Release configuration
* transfers the publish output as a deployment artifact
* authenticates to Azure using federated OIDC credentials
* deploys the artifact to Azure App Service

Azure authentication therefore does not require a deployment password to be stored in the repository.

### Live Azure Deployment

Swagger / OpenAPI documentation is available at:

[WeatherGatewayAPI on Azure App Service](https://weathergatewayapi-a8hqgeavazg6a5c3.italynorth-01.azurewebsites.net/swagger)

The production API remains protected by JWT authorization; the Development-only token endpoint is not exposed in Azure.

---

## Technology Stack

**Backend**
C# · .NET 10 · ASP.NET Core Web API · REST

**Integration**
Typed HttpClient · JSON · DTO Mapping · Options Pattern

**Security**
JWT Bearer Authentication · Authorization · User Secrets

**Diagnostics**
Structured Logging · Custom Exception Middleware

**API Documentation**
Swagger · OpenAPI

**Cloud & Delivery**
GitHub Actions · CI/CD · OIDC · Azure App Service

---

## Design Scope

WeatherGatewayAPI is intentionally designed as a **focused external-service gateway**, rather than a complete identity or distributed integration platform.

Key implementation choices include:

* integration with one external weather provider;
* typed `HttpClient` communication with a configurable timeout;
* DTO mapping between external and internal contracts;
* JWT-protected endpoints without a persistent user store;
* Development-only token generation for local authorization testing;
* centralized exception handling and structured logging;
* automated Azure App Service deployment through GitHub Actions.

A larger production integration platform could extend these areas with retry and circuit-breaker policies, caching, persistent identity, rate limiting, richer observability, integration testing, and multiple external providers.

---

## License

This project is licensed under the [MIT License](LICENSE).
