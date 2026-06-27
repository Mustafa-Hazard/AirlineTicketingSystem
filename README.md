# ✈️ AeroNexa — Airline Ticketing System

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet)
![Entity Framework Core](https://img.shields.io/badge/EF%20Core-ORM-512BD4)
![SQL Server](https://img.shields.io/badge/MS%20SQL%20Server-Database-CC2927?logo=microsoftsqlserver&logoColor=white)
![C#](https://img.shields.io/badge/C%23-Language-239120?logo=csharp&logoColor=white)
![Status](https://img.shields.io/badge/status-active-brightgreen)

A role-based airline reservation platform built on ASP.NET Core 8 MVC, with separate **Admin** and **Passenger** experiences, secure session authentication, full booking lifecycle management, automated PDF e-ticket generation, and SQL-driven revenue analytics.

---

## ✨ Features

| Feature | Description |
|---|---|
| 🔐 **Role-Based Access** | Separate Admin and Passenger portals with secure session-based authentication |
| 🎫 **Booking Lifecycle** | End-to-end flow — search, book, confirm, cancel, and manage reservations |
| 📄 **Automated E-Tickets** | PDF tickets generated automatically on successful booking |
| 📊 **Revenue Analytics** | Admin dashboard powered by optimized SQL stored procedures |
| ✅ **Validated Workflows** | Booking logic tested using equivalence partitioning and boundary value analysis |

---

## 🏗️ Tech Stack

- **Framework:** ASP.NET Core 8 MVC
- **ORM:** Entity Framework Core
- **Database:** MS SQL Server
- **Language:** C#
- **Architecture:** MVC pattern with stored-procedure-backed analytics

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MS SQL Server (local instance or Docker container)
- Visual Studio 2022+ or VS Code with the C# extension

### Setup

```bash
git clone https://github.com/Mustafa-Hazard/AeroNexa.git
cd AeroNexa
```

**1. Configure the database connection**

Update `appsettings.json` with your SQL Server connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=AeroNexaDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**2. Apply migrations**
```bash
dotnet ef database update
```

**3. Run the application**
```bash
dotnet run
```

**4. Open in browser**

Navigate to `https://localhost:5001` (or whatever port the console output shows).

---

## 👤 Roles & Access

| Role | Capabilities |
|---|---|
| **Admin** | Manage flights, view revenue analytics, oversee all bookings |
| **Passenger** | Search flights, book tickets, view/download e-tickets, manage own reservations |

---

## 🧪 Testing Approach

Booking workflows were validated using:
- **Equivalence Partitioning** — grouping valid/invalid input ranges (e.g. seat counts, dates) to test representative cases
- **Boundary Value Analysis** — testing edge cases at the limits of valid input ranges (e.g. minimum/maximum passengers per booking)

---

## 📌 Roadmap Ideas

- [ ] Payment gateway integration
- [ ] Email notifications for booking confirmations
- [ ] Multi-currency pricing support
- [ ] Dockerized deployment

---

## 👨‍💻 Author

**Mustafa Muhammad Iqbal**
[LinkedIn](https://linkedin.com/in/mustafa642) · [GitHub](https://github.com/Mustafa-Hazard)
