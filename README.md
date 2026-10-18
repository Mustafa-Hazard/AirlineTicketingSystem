# AeroNexa — Airline Ticketing System

Role-based airline booking system built with ASP.NET Core 8 MVC, Entity Framework Core, dual database support (SQL Server & PostgreSQL), and automated PDF e-tickets.

![AeroNexa Preview](docs/aeronexa.png)

## About
AeroNexa is a web application designed to handle flight reservations and scheduling. It separates user workflows into dedicated Admin and Passenger experiences using session-based authentication and JWT cookie verification. Passengers can search for flights, book seats, view booking confirmations, and download generated PDF e-tickets. Administrators have access to flight management tools and dashboard analytics to monitor revenue and booking volumes.


## Features
- **Passenger Portal:** Search available flights, select seat classes, enter passenger details, and confirm ticket bookings.
- **Automated PDF E-Tickets:** Generates downloadable PDF boarding passes and e-tickets on booking confirmation using QuestPDF (`Pdf/ETicketDocument.cs`).
- **Email Notifications:** Email service interface and implementation using MailKit and MimeKit (`Services/Interfaces/Implementation/EmailService.cs`).
- **Admin Management Portal:** Create, update, and cancel flight routes, manage flight schedules, and view all system reservations.
- **Revenue Analytics:** Admin dashboard displays booking metrics and sales figures calculated through SQL database queries.
- **Dual Database Architecture:** Supports Microsoft SQL Server for local development and PostgreSQL for production deployments.

## Tech Stack
*Stack verified from `AirlineTicketingSystem.csproj`, `Program.cs`, and `appsettings.json`.*

| Layer | Technologies |
|---|---|
| **Framework & Language** | ASP.NET Core 8 MVC, C# (.NET 8.0) |
| **ORM & Databases** | Entity Framework Core 8.0, Microsoft SQL Server (`Microsoft.EntityFrameworkCore.SqlServer` for local dev), PostgreSQL (`Npgsql.EntityFrameworkCore.PostgreSQL` for production) |
| **PDF Generation** | QuestPDF 2025.7.4 |
| **Auth & Security** | BCrypt.Net-Core 1.6, Microsoft.AspNetCore.Authentication.JwtBearer 8.0 |
| **Email Services** | MailKit 4.14, MimeKit 4.14 |
| **Deployment** | Dockerfile, Railway (`railway.json`) |

Live application: [https://aeronexa.up.railway.app](https://aeronexa.up.railway.app)

## Getting Started

### Prerequisites
- .NET 8 SDK
- Microsoft SQL Server (local) OR PostgreSQL

### 1. Clone the Repository
```bash
git clone https://github.com/Mustafa-Hazard/AirlineTicketingSystem.git
cd AirlineTicketingSystem
```

### 2. Configure Database
By default, the application runs on Microsoft SQL Server via `appsettings.json`. Update `ConnectionStrings:DefaultConnection`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AeroNexaDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "UsePostgreSQL": false
}
```
*To use PostgreSQL locally, set `"UsePostgreSQL": true` or provide a `DATABASE_URL` environment variable (the format used automatically when deployed to Railway).*

### 3. Run Migrations & Start Application
```bash
dotnet ef database update
dotnet run
```
The application will launch on `https://localhost:5001` or `http://localhost:5000`.

### Environment / Configuration Variables

| Key | Environment Variable | Description | Example Placeholder |
|---|---|---|---|
| `ConnectionStrings:DefaultConnection` | `DATABASE_URL` | Database connection string (SQL Server or PostgreSQL URL) | `Server=localhost;Database=AeroNexaDb;Trusted_Connection=True;` |
| `UsePostgreSQL` | N/A | Boolean flag to switch provider to PostgreSQL | `false` |
| `Jwt:Issuer` | `JWT_ISSUER` | Token issuer identifier | `https://aeronexa.up.railway.app/` |
| `Jwt:Audience` | `JWT_AUDIENCE` | Token audience identifier | `https://aeronexa.up.railway.app/` |
| `Jwt:Key` | `JWT_KEY` | Secret key for JWT signing (minimum 32 characters) | `your_secret_key_minimum_32_characters` |
| `EmailSettings:SmtpServer` | N/A | SMTP host for email dispatch | `smtp.example.com` |
| `EmailSettings:Username` | N/A | SMTP account username | `user@example.com` |
| `EmailSettings:Password` | N/A | SMTP account password or app password | `your_email_password` |

## Project Structure
```text
AirlineTicketingSystem/
├── Controllers/       # Admin, Booking, Flight, Passenger, and Splash MVC controllers
├── Data/              # DatabaseContext, DbInitializer, and entity configurations
├── Migrations/        # Entity Framework Core migrations
├── Models/            # Domain models (Flight, Booking, Passenger, User, Settings)
├── Pdf/               # QuestPDF document layout (ETicketDocument.cs)
├── Services/          # MailKit email service (EmailService.cs)
├── Views/             # Razor view templates for Admin, Passenger, and Booking flows
├── Program.cs         # Service registration, DB provider routing, and auth middleware
└── railway.json       # Railway deployment specification
```

## What I Learned & What I Would Improve
- Configured dynamic runtime database selection in `Program.cs` to switch between SQL Server (local development) and PostgreSQL (Railway deployment).
- Implemented automated PDF e-ticket generation with QuestPDF and connected email confirmations via MailKit.
- Future improvements: integrate an external payment gateway, implement seat map selection, and add automated integration test coverage.

## Author
**Mustafa Muhammad Iqbal** — [GitHub](https://github.com/Mustafa-Hazard) · [LinkedIn](https://linkedin.com/in/mustafa642)
