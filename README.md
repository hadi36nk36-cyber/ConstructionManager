```markdown
# ConstructionManager API 🏗️

A robust, enterprise-grade REST API for managing construction projects, customers, employees, and financials. Built with a focus on clean architecture, security, and testability.

## 🛠️ Tech Stack

- **Framework:** .NET 10, ASP.NET Core Web API
- **Database:** PostgreSQL 17 (via Entity Framework Core)
- **Architecture:** Practical Clean Architecture
- **Security:** JWT Authentication, Refresh Tokens, Rate Limiting (IP-based & Endpoint-specific)
- **Testing:** xUnit, Moq (52 Unit Tests)
- **Containerization:** Docker, Docker Compose
- **Logging:** Serilog
- **API Docs:** Scalar (OpenAPI)

## 🏗️ Architecture (Clean Architecture)

The solution is divided into 4 main layers:
- **Domain:** Core entities, enums, value objects, and custom exceptions.
- **Application:** Business logic, services, DTOs, interfaces, and mappings.
- **Infrastructure:** Database context, repositories, JWT services, and external tools (BCrypt).
- **Presentation:** API controllers, middlewares, and validators.

## 🚀 Getting Started (Docker)

The easiest way to run this project is using Docker. You don't even need to install .NET or PostgreSQL on your machine!

1. Clone the repository.
2. Open your terminal in the root directory.
3. Run the following command:

```bash
docker-compose up -d --build
```

4. The API will be available at: `http://localhost:7000/scalar/v1`
5. The default admin user will be seeded automatically:
   - **Username:** `admin`
   - **Password:** `123`

## 🧪 Running Tests

To run the unit tests locally:

```bash
dotnet test
```

## 📌 Features Implemented

- **CRUD** operations for 12 entities (Customers, Projects, Employees, Invoices, etc.)
- **JWT Authentication** with secure password hashing (BCrypt).
- **Refresh Token** mechanism for seamless user experience.
- **Role-Based Authorization** (Admin/User).
- **Global & Endpoint-specific Rate Limiting** to prevent brute-force and DDoS attacks.
- **Defensive Programming** with custom exceptions and a global Exception Middleware.
- **Auto Database Migration** and **Data Seeding** on application startup.

---
*This document was drafted with the assistance of AI.*
```