# CushyPay - Next Gen Banking & Payment API

A robust, scalable, and secure backend API for a Fintech project built with .NET 8, ASP.NET Core Web API, and MSSQL.

## Architecture

This project follows **Clean Architecture** (Onion Architecture) and **Domain-Driven Design (DDD)** principles:

- **Domain Layer**: Core business logic, entities, value objects (No dependencies)
- **Application Layer**: CQRS with MediatR, DTOs, Validators, Mappings
- **Infrastructure Layer**: EF Core, MSSQL, External Services
- **API Layer**: Controllers, Middleware, DI Container

## Features

### ✅ Implemented

- **User Management & Authentication**
  - User Registration (Standard & Merchant roles)
  - Login with JWT Token generation
  - Refresh Token mechanism
  - Password hashing with BCrypt

- **Wallet/Account Management**
  - Multiple wallets per user (Main Account, Savings, Dollar Account, etc.)
  - Currency Support: TRY and USD
  - Auto-generated unique IBANs for each wallet

- **Transaction System**
  - Internal Transfer: Send money between users (Instant)
  - External Transfer: Send money to external bank accounts (Async simulation)
  - Deposit: Add funds via Credit Card (Mock)
  - Withdrawal: Withdraw to bank account
  - Transaction History with filtering (Date range, Amount, Type, Status)

- **Security & Concurrency**
  - Optimistic Concurrency Control (RowVersion) on Wallet entity
  - Unit of Work pattern for atomic transactions
  - Automatic Audit Logging
  - Immutable Transactions (Append-only)

- **Validation & Error Handling**
  - FluentValidation for all commands
  - Global Exception Handling Middleware
  - Consistent error responses

## Technology Stack

- **.NET 8** (LTS)
- **ASP.NET Core Web API**
- **Entity Framework Core 8.0** (Code-First)
- **MSSQL Server**
- **MediatR** (CQRS pattern)
- **FluentValidation**
- **AutoMapper**
- **JWT Bearer Authentication**
- **BCrypt.Net-Next**
- **Swagger/OpenAPI**

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (Express or higher)
- Visual Studio 2022 or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd CushyPay
   ```

2. **Update Connection String**
   Edit `appsettings.json` and update the `DefaultConnection` string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your SQL Server connection string"
   }
   ```

3. **Run Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access Swagger UI**
   Navigate to: `https://localhost:5001/swagger` (or the port shown in console)

## API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh-token` - Refresh access token

### Wallets (`/api/wallets`)
- `POST /api/wallets` - Create a new wallet (Requires Authentication)
- `GET /api/wallets/user/{userId}` - Get all wallets for a user (Requires Authentication)

### Transactions (`/api/transactions`)
- `POST /api/transactions/internal-transfer` - Transfer between CushyPay users
- `POST /api/transactions/external-transfer` - Transfer to external bank
- `POST /api/transactions/deposit` - Deposit funds
- `POST /api/transactions/withdraw` - Withdraw funds
- `GET /api/transactions/history/{walletId}` - Get transaction history with filters

## Database Schema

- **Users**: User accounts with authentication
- **Wallets**: User wallets with IBAN and balance (RowVersion for concurrency)
- **Transactions**: Immutable transaction records
- **AuditLogs**: Automatic change tracking
- **RefreshTokens**: JWT refresh token storage

## Key Business Rules

1. **Concurrency Control**: Wallet uses RowVersion to prevent double-spending
2. **Immutability**: Transactions are append-only (never updated or deleted)
3. **Atomicity**: Unit of Work ensures transaction atomicity
4. **Auditing**: All entity changes are automatically logged
5. **Validation**: Domain entities and FluentValidation enforce business rules

## Future Enhancements

- [ ] Background Jobs for processing scheduled payments (Hangfire/IHostedService)
- [ ] Rate Limiting implementation
- [ ] Email/SMS notification service implementation
- [ ] Additional currency support
- [ ] Transaction webhooks
- [ ] Advanced reporting and analytics

## License

This project is for portfolio/educational purposes.

