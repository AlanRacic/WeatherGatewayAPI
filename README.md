# WeatherGatewayAPI — ASP.NET Core Web API with External Service Integration (.NET 10)

## Overview
WeatherGatewayAPI is an ASP.NET Core Web API that acts as a gateway to an external weather service, demonstrating secure API design, external service integration, and production-oriented backend patterns.

The project implements JWT-based authentication, structured logging, DTO-based data transformation, and global exception handling. It showcases how a backend service receives client requests, communicates with an external API, and returns structured, reliable responses.

The architecture emphasizes clean separation of concerns, predictable error handling, and maintainable service orchestration aligned with modern .NET backend practices.

---

## Tech Stack

**Core**  
C# · .NET 10 · ASP.NET Core Web API

**Integration & Data Handling**  
HttpClient · JSON · DTO Mapping

**Security & Infrastructure**  
JWT Authentication · Authorization

**Architecture & Practices**  
Dependency Injection · Middleware · Logging · Options Pattern · Async/Await

---

## Key Features

- JWT-based authentication with token generation and secured endpoints
- Protected API endpoints using `[Authorize]` and JWT validation
- External API integration via HttpClient with configurable base URL and timeout
- DTO-based transformation of external service responses into internal models
- Structured logging using `ILogger` for request tracking and diagnostics
- Global exception handling via custom middleware with consistent JSON responses
- Input validation with clear error messaging for predictable API behavior
- Separation of concerns between controllers, services, and data contracts

---

## Architecture Notes

- Controller layer handles HTTP requests, validation, and response formatting
- Service layer encapsulates external API communication and business logic
- DTOs define boundaries between external responses and internal API contracts
- Options pattern is used for strongly typed configuration (API settings, timeouts, keys)
- Middleware centralizes exception handling and standardizes error responses
- HttpClient is configured via dependency injection for reliability and reuse

This project reflects a gateway-style architecture, where the API acts as an intermediary between clients and external services.

---

## What This Project Demonstrates

- Designing a secure Web API with JWT authentication in ASP.NET Core
- Integrating external services using HttpClient and configuration-based setup
- Handling errors across service and middleware layers with consistent responses
- Applying DTO mapping to decouple external and internal data models
- Implementing structured logging for observability and debugging
- Using dependency injection and options pattern for clean configuration management

---

## How to Run (Local Setup)

### Prerequisites
- .NET SDK 10
- (Optional) Visual Studio 2022 or Rider

### Steps

Clone the repository

```bash
git clone https://github.com/alanracic/WeatherGatewayAPI.git
```

Update configuration in `appsettings.json`
- Set external API base URL
- Provide a valid API key
- Configure JWT settings if needed

Run the application

```bash
dotnet run
```

Use Swagger / OpenAPI to test endpoints (in development environment)

---

## Project Structure (High-Level)

- Controllers — API endpoints (Auth, Weather)
- Service — external service integration and business logic
- Dtos — request/response data contracts
- Middleware — global exception handling
- Program.cs — application configuration and service wiring

---

## Skills Demonstrated

ASP.NET Core Web API · C# · .NET 10 · JWT Authentication · HttpClient · External API Integration · DTO Mapping · Middleware · Logging · Dependency Injection · Options Pattern · Async/Await

---

## Project Status

Actively maintained as part of a professional .NET portfolio, demonstrating external service integration, secure API design, and clean backend architecture patterns suitable for real-world applications.
