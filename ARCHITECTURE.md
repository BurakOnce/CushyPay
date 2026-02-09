# CushyPay - Clean Architecture Implementation

## Project Structure

The project follows Clean Architecture (Onion Architecture) principles with clear separation of concerns:

```
CushyPay/
├── Domain/                    # Core business logic (No dependencies)
│   ├── Entities/             # Domain entities
│   ├── ValueObjects/         # Immutable value objects
│   ├── Enums/                # Domain enumerations
│   ├── Exceptions/           # Domain-specific exceptions
│   └── Common/               # Base classes
│
├── Application/              # Application logic (Depends on Domain)
│   ├── Common/
│   │   ├── Interfaces/       # Application interfaces
│   │   └── Mappings/         # AutoMapper profiles
│   └── Features/             # Feature-based organization (to be implemented)
│
├── Infrastructure/           # External concerns (Depends on Domain & Application)
│   ├── Data/                 # EF Core DbContext & configurations
│   └── Services/             # External services (to be implemented)
│
└── Controllers/              # API layer (Depends on Application)
```

## Domain Layer Entities

### 1. **User** (`Domain/Entities/User.cs`)
- Represents a user in the system
- Supports Standard and Merchant roles
- Contains authentication information (Email, PasswordHash)
- Has navigation properties to Wallets and RefreshTokens

### 2. **Wallet** (`Domain/Entities/Wallet.cs`)
- Represents a user's wallet/account
- Supports multiple currencies (TRY, USD)
- **Optimistic Concurrency Control**: Uses `RowVersion` (byte[]) to prevent race conditions
- Auto-generates unique IBAN upon creation
- Contains business logic for Credit/Debit operations

### 3. **Transaction** (`Domain/Entities/Transaction.cs`)
- **Immutable, Append-Only**: Once created, transactions cannot be updated or deleted
- Supports multiple transaction types:
  - InternalTransfer: Between CushyPay users
  - ExternalTransfer: To external bank accounts
  - Deposit: Via credit card (mock)
  - Withdrawal: To bank account
- Tracks transaction status (Pending, Completed, Failed, Cancelled)
- Generates unique reference numbers

### 4. **AuditLog** (`Domain/Entities/AuditLog.cs`)
- Tracks all entity changes (Created, Updated, Deleted)
- Stores change details as JSON
- Captures user information and IP address

### 5. **RefreshToken** (`Domain/Entities/RefreshToken.cs`)
- Stores JWT refresh tokens
- Supports token revocation
- Tracks expiration and usage

## Value Objects

### 1. **Iban** (`Domain/ValueObjects/Iban.cs`)
- Immutable IBAN value object
- Validates IBAN format
- Generates unique IBANs

### 2. **Money** (`Domain/ValueObjects/Money.cs`)
- Type-safe money representation
- Currency-aware operations (Add, Subtract, Compare)
- Prevents mixing different currencies

## Enums

- **UserRole**: Standard, Merchant
- **Currency**: TRY, USD
- **TransactionType**: InternalTransfer, ExternalTransfer, Deposit, Withdrawal
- **TransactionStatus**: Pending, Completed, Failed, Cancelled

## Infrastructure Layer

### DbContext (`Infrastructure/Data/AppDbContext.cs`)
- Implements automatic audit logging via `SaveChangesAsync` override
- Tracks entity changes and creates AuditLog entries
- Supports setting current user context for audit trails

### Entity Configurations
- **UserConfiguration**: Email uniqueness, relationships
- **WalletConfiguration**: IBAN uniqueness, RowVersion for concurrency
- **TransactionConfiguration**: Indexes for performance
- **AuditLogConfiguration**: Optimized for querying
- **RefreshTokenConfiguration**: Token management

## Key Features Implemented

✅ **Clean Architecture Structure**
✅ **Domain-Driven Design Entities**
✅ **Optimistic Concurrency Control** (RowVersion on Wallet)
✅ **Audit Logging** (Automatic via SaveChangesAsync)
✅ **Immutable Transactions** (Append-only)
✅ **Value Objects** (Iban, Money)
✅ **Entity Configurations** (EF Core Fluent API)

## Next Steps (To Be Implemented)

1. **Application Layer**:
   - CQRS with MediatR (Commands & Queries)
   - DTOs for API communication
   - FluentValidation validators
   - AutoMapper profiles

2. **Infrastructure Services**:
   - JWT Authentication service
   - Password hashing (BCrypt)
   - Email service interface
   - Unit of Work implementation

3. **API Controllers**:
   - User registration/login
   - Wallet management
   - Transaction processing
   - Transaction history

4. **Database Migration**:
   - Run `dotnet ef migrations add InitialCreate`
   - Run `dotnet ef database update`

## Database Schema Overview

- **Users**: User accounts with authentication
- **Wallets**: User wallets with IBAN and balance
- **Transactions**: Immutable transaction records
- **AuditLogs**: Change tracking
- **RefreshTokens**: JWT refresh token storage

## Business Rules Enforced

1. **Concurrency**: Wallet uses RowVersion to prevent double-spending
2. **Immutability**: Transactions are append-only
3. **Atomicity**: Unit of Work pattern ensures transaction atomicity
4. **Auditing**: All changes are automatically logged
5. **Validation**: Domain entities validate business rules

## Package Dependencies

- Entity Framework Core 8.0.0
- MediatR 12.2.0
- FluentValidation 11.9.0
- AutoMapper 12.0.1
- JWT Bearer Authentication 8.0.0
- BCrypt.Net-Next 4.0.3
- Swashbuckle (Swagger) 6.4.0

