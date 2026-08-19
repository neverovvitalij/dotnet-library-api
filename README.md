# Library API

A REST API for managing a library — books, authors, genres, and loans — built with **ASP.NET Core** following **Clean Architecture**. Started as a structured learning project to practice production-grade backend patterns (Repository, CQRS, JWT auth, API versioning) and grew into a fully working, tested service.

## Features

- **Full CRUD** for Books, Authors, Genres, and Loans, with proper REST semantics (201 + Location on create, 204 on delete, 404/409 where appropriate).
- **Business rules enforced at the database level:** a book cannot have two simultaneous active loans — protected by a unique partial index (`WHERE "ReturnDate" IS NULL`), in addition to an application-level check, guarding against race conditions on concurrent requests.
- **JWT Authentication:** short-lived access tokens (with role claims) + long-lived refresh tokens, stored server-side with **token rotation** (old refresh token is revoked on every refresh). Passwords are hashed via `PasswordHasher<T>` (PBKDF2, salted).
- **Role-based authorization:** `Admin`-only endpoints for creating/editing/deleting books, authors, genres, and for issuing/returning loans; read endpoints are public.
- **User enumeration protection:** login returns an identical, generic error for "user not found" and "wrong password".
- **API versioning:** `/api/v1/books` and `/api/v2/books` (v2 adds a `Publisher` field), with separate Swagger documents per version.
- **Pagination:** `page`/`pageSize` query parameters with total count returned via the `X-Total-Count` response header.
- **CQRS with MediatR** for selected operations (`GetBooks`, `CreateBook`, `CreateLoan`), including a custom **Pipeline Behavior** that logs the execution time of every request.
- **Unit tests** (xUnit + Moq) covering the loan business logic in isolation from the database.

## Architecture

The solution is split into four projects, following the Clean Architecture dependency rule (dependencies point inward only):

```
Domain          → plain entities, no external dependencies
Application     → repository interfaces, DTOs, CQRS commands/queries & handlers
Infrastructure  → EF Core DbContext, repository implementations, JWT token service
API             → controllers, request/response DTOs, composition root
```

Controllers depend only on repository interfaces (or `IMediator` for the CQRS-based endpoints) — never on EF Core directly. This keeps business logic testable without a real database and decoupled from the storage technology.

## Tech Stack

- **Framework:** ASP.NET Core 8, C#
- **Database:** PostgreSQL (EF Core, code-first migrations)
- **Auth:** JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`), `Microsoft.AspNetCore.Identity` password hashing
- **CQRS:** MediatR
- **API Docs:** Swagger / OpenAPI (Swashbuckle), versioned via `Asp.Versioning`
- **Testing:** xUnit, Moq
- **Config:** strongly-typed options via `IOptions<T>`, secrets via User Secrets (never committed)

## Entities & Relations

- `Author` 1—* `Book`
- `Book` *—* `Genre`
- `Book` 1—* `Loan`
- `User` 1—* `RefreshToken`

## API Overview

| Endpoint | Auth |
|---|---|
| `GET /api/v{version}/books` | Public |
| `POST /api/v{version}/books` | Admin |
| `PUT /api/v{version}/books/{id}` | Admin |
| `DELETE /api/v{version}/books/{id}` | Admin |
| `GET /api/authors`, `/api/genres` | Public |
| `POST/PUT/DELETE /api/authors`, `/api/genres` | Admin |
| `GET /api/loans` | Admin |
| `POST /api/loans` | Admin |
| `PUT /api/loans/{id}/return` | Admin |
| `POST /api/auth/register`, `/login`, `/refresh` | Public |

## Getting Started

```bash
git clone https://github.com/neverovvitalij/dotnet-library-api
cd dotnet-library-api
```

Set the following via User Secrets (or environment variables) in the API project:

```json
{
  "ConnectionStrings": {
    "LibraryDb": "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require"
  },
  "Jwt": {
    "Key": "a long random secret",
    "Issuer": "dotnet-library-api",
    "Audience": "dotnet-library-api-users",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

Apply migrations and run:

```bash
dotnet ef database update --project dotnet-library-api.Infrastructure --startup-project dotnet-library-api
dotnet run --project dotnet-library-api
```

Swagger UI is available at `/swagger` in Development.

## Running Tests

```bash
dotnet test
```
