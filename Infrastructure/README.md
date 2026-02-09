# Infrastructure Layer

This layer contains:
- **Data**: Entity Framework Core DbContext and configurations
- **Services**: External service implementations (Email, SMS, etc.)
- **Repositories**: Data access implementations
- **Identity**: JWT authentication and authorization

## Structure

```
Infrastructure/
├── Data/
│   ├── AppDbContext.cs
│   └── Configurations/   # EF Core entity configurations
├── Services/             # External service implementations
├── Repositories/         # Repository implementations
├── Identity/            # JWT and authentication
└── README.md
```

