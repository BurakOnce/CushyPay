# Domain Layer

This layer contains the core business logic and entities following Domain-Driven Design (DDD) principles.

## Structure

```
Domain/
├── Entities/         # Domain entities (User, Wallet, Transaction, etc.)
├── ValueObjects/     # Immutable value objects (Iban, Money)
├── Enums/           # Domain enumerations
├── Exceptions/      # Domain-specific exceptions
├── Common/          # Base classes and shared domain logic
└── README.md
```

## Key Principles

- **No Dependencies**: This layer has no dependencies on other layers
- **Rich Domain Model**: Entities contain business logic, not just data
- **Immutability**: Value objects and transactions are immutable
- **Concurrency Control**: Wallet entity uses RowVersion for optimistic concurrency

