# Order Management System

A comprehensive order management system built with **.NET 9**, **Clean Architecture**, **CQRS**, **Domain-Driven Design**, and **Entity Framework Core**.

## 🏗️ Architecture Overview

This project demonstrates enterprise-level software architecture patterns:

* **Clean Architecture** with clear separation of concerns
* **CQRS** (Command Query Responsibility Segregation) for scalable operations
* **Domain-Driven Design** with rich domain models and business logic
* **Specification Pattern** for flexible and reusable queries
* **Entity Framework Core** with advanced features like transactions and migrations
* **Domain Events** for decoupled business logic
* **Result Pattern** for explicit error handling
* **Repository Pattern** with Unit of Work for data access abstraction

## 🚀 Features

### Core Functionality

* **Customer Management**: Create and manage customer profiles
* **Product Catalog**: Manage products with pricing, inventory, and categories
* **Order Processing**: Complete order lifecycle from creation to delivery
* **Inventory Management**: Real-time stock tracking with low-stock alerts
* **Transaction Support**: Database transactions ensure data consistency

### Technical Features

* **RESTful API** with comprehensive Swagger documentation
* **Validation** using FluentValidation with pipeline behaviors
* **Logging** with Serilog and structured logging
* **Health Checks** for monitoring and load balancer integration
* **Error Handling** with global exception middleware
* **Performance Monitoring** with request timing
* **Docker Support** with multi-stage builds and health checks

## 🛠️ Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (for local development)
* [Git](https://git-scm.com/)

### Optional Tools

* [Visual Studio 2022](https://visualstudio.microsoft.com/) or [JetBrains Rider](https://www.jetbrains.com/rider/)
* [Postman](https://www.postman.com/) or [Insomnia](https://insomnia.rest/) for API testing

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/Bo-Ayoub/order-management-system.git
cd order-management-system
```

### 2. Run with Docker (Recommended)

```bash
# Start the entire application stack
docker-compose up -d

# Check if services are running
docker-compose ps

# View logs
docker-compose logs -f ordermanagement-api
```

The API will be available at:

* **Swagger UI**: [http://localhost:5000](http://localhost:5000)
* **Health Check**: [http://localhost:5000/health](http://localhost:5000/health)
* **Database**: SQL Server on localhost:1433

### 3. Run Locally (Development)

#### Setup Database

```bash
# Update connection string in appsettings.Development.json if needed
# Run migrations to create database
cd src/OrderManagement.API
dotnet ef database update
```

#### Run Application

```bash
# Restore dependencies
dotnet restore

# Run the API
cd src/OrderManagement.API
dotnet run

# Or run with hot reload
dotnet watch run
```

The API will be available at:

* **HTTPS**: [https://localhost:7001](https://localhost:7001)
* **HTTP**: [http://localhost:5000](http://localhost:5000)
* **Swagger**: [https://localhost:7001](https://localhost:7001) or [http://localhost:5000](http://localhost:5000)

## 🧪 Testing

### Run Unit Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/OrderManagement.UnitTests/

# Run tests in watch mode
dotnet watch test
```

## 📚 API Documentation

### Authentication

This demo version doesn't include authentication. In production, you would add:

* JWT authentication
* API key validation
* OAuth 2.0 / OpenID Connect

### Core Endpoints

#### Customers

* `GET /api/customers` - Get customers with pagination and search
* `GET /api/customers/{id}` - Get customer by ID
* `POST /api/customers` - Create new customer

#### Products

* `GET /api/products` - Get products with filtering and pagination
* `GET /api/products/{id}` - Get product by ID
* `POST /api/products` - Create new product

#### Orders

* `GET /api/orders` - Get orders with filtering and pagination
* `GET /api/orders/{id}` - Get order details
* `POST /api/orders` - Create new order
* `POST /api/orders/{id}/confirm` - Confirm order
* `PUT /api/orders/{id}/status` - Update order status

#### System

* `GET /health` - Detailed health check
* `GET /health/ready` - Readiness probe
* `GET /health/live` - Liveness probe

### Example API Calls

#### Create Customer

```bash
curl -X POST http://localhost:5000/api/customers \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "+1-555-0123"
  }'
```

#### Create Product

```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Wireless Headphones",
    "price": 199.99,
    "stockQuantity": 50,
    "description": "Premium noise-cancelling headphones",
    "category": "Electronics"
  }'
```

#### Create Order

```bash
curl -X POST http://localhost:5000/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "customer-guid-here",
    "items": [
      {
        "productId": "product-guid-here",
        "quantity": 2
      }
    ],
    "shippingAddress": "123 Main St, Anytown, USA",
    "notes": "Please handle with care"
  }'
```

## 🏛️ Architecture Principles Explained

### Clean Architecture Layers

**Domain Layer (Core)**

* Contains business entities, value objects, and domain events
* No dependencies on external frameworks
* Encapsulates business rules and invariants

**Application Layer**

* Orchestrates domain operations
* Implements CQRS commands and queries
* Contains validation and business workflows
* Depends only on Domain layer

**Infrastructure Layer**

* Implements data persistence with Entity Framework Core
* Contains repository implementations and external service integrations
* Depends on Application and Domain layers

**API Layer**

* Handles HTTP requests and responses
* Contains controllers, middleware, and configuration
* Depends on Application and Infrastructure layers

### Key Patterns Implemented

**CQRS (Command Query Responsibility Segregation)**

* Commands: Modify state (CreateOrder, ConfirmOrder)
* Queries: Return data (GetOrder, GetOrders)
* Separate models optimize for different use cases

**Repository + Specification Pattern**

```csharp
// Flexible querying without repository explosion
var orders = await _orderRepository.FindAsync(
    new OrdersByCustomerSpecification(customerId));
```

**Domain Events**

```csharp
// Decoupled business reactions
public void Confirm()
{
    Status = OrderStatus.Confirmed;
    AddDomainEvent(new OrderStatusChangedEvent(Id, previousStatus, Status));
}
```

**Value Objects**

```csharp
// Encapsulated business rules
public class Money : ValueObject
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return new Money(Amount + other.Amount, Currency);
    }
}
```

**Result Pattern**

```csharp
// Explicit error handling
public async Task<Result<Guid>> Handle(CreateOrderCommand request)
{
    if (customer == null)
        return Result<Guid>.Failure("Customer not found");
    
    return Result<Guid>.Success(order.Id);
}
```

## 🔧 Configuration

### Database Configuration

The application supports multiple database providers:

**SQL Server (Default)**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderManagementDb;Trusted_Connection=true"
  }
}
```

**Docker SQL Server**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=OrderManagementDb;User Id=sa;Password=OrderManagement123!;TrustServerCertificate=true"
  }
}
```

### Environment Variables

```bash
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection="your-connection-string"
EnableSensitiveDataLogging=true
EnableDetailedErrors=true
```

## 🐳 Docker Commands

### Development

```bash
# Build and start services
docker-compose up --build

# Start in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Clean up volumes
docker-compose down -v
```

### Production

```bash
# Build production image
docker build -f src/OrderManagement.API/Dockerfile -t order-management:latest .

# Run production container
docker run -d \
  --name order-management \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="your-production-connection" \
  order-management:latest
```

## 🎯 Business Rules Implemented

### Order Management

* Orders must have at least one item before confirmation
* Stock is reserved when order is created
* Orders can only be modified in Pending status
* Status transitions follow business workflow
* Shipping address required for confirmation

### Inventory Management

* Stock is updated when orders are created
* Products with zero stock cannot be ordered
* Low stock alerts when inventory falls below threshold
* Products can be activated/deactivated

### Customer Management

* Email addresses must be unique
* Customer information validation
* Customer order history tracking

## 🔍 Monitoring and Observability

### Logging

* Structured logging with Serilog
* Request/response logging
* Performance monitoring (slow request detection)
* Error tracking with correlation IDs

### Health Checks

* Database connectivity check
* Custom business logic health checks
* Kubernetes-ready liveness and readiness probes

### Metrics (Future Enhancement)

Consider adding:

* Application metrics with Prometheus
* Distributed tracing with OpenTelemetry
* Performance counters

## 🛡️ Security Considerations

### Current Implementation

* Input validation with FluentValidation
* SQL injection prevention via EF Core parameterized queries
* CORS configuration
* HTTPS enforcement
* Security headers

### Production Recommendations

* Add authentication (JWT, OAuth)
* Implement authorization policies
* Rate limiting
* API versioning
* Input sanitization
* Audit logging

## 🚀 Deployment

### Local Development

1. Install .NET 9 SDK
2. Run `dotnet restore`
3. Update connection string
4. Run `dotnet ef database update`
5. Run `dotnet run`

### Docker Deployment

1. Run `docker-compose up`
2. Access API at [http://localhost:5000](http://localhost:5000)

### Cloud Deployment

* **Azure**: Use Azure Container Instances or Azure App Service
* **AWS**: Use ECS or Elastic Beanstalk
* **Google Cloud**: Use Cloud Run or GKE

## 🧪 Testing Strategy

### Unit Tests

* Domain entity behavior testing
* Value object validation testing
* Command/Query handler testing
* Validation rule testing

### Architecture Tests

Consider adding:

* Dependency rule enforcement
* Naming convention validation
* Layer isolation verification

## 📈 Performance Considerations

### Database Optimization

* Proper indexing strategy
* Query optimization with specifications
* Connection pooling
* Read replicas for queries (future)

### Caching Strategy (Future Enhancement)

* Redis for distributed caching
* Response caching for queries
* Memory caching for reference data

### Scalability

* CQRS enables read/write separation
* Stateless API design
* Database connection pooling
* Horizontal scaling ready

## 🛠️ Development Guidelines

### Code Standards

* Follow Clean Code principles
* Use meaningful names and small methods
* Comprehensive error handling
* Extensive unit test coverage

### Git Workflow

```bash
# Feature development
git checkout -b feature/new-feature
git commit -m "feat: add new feature"
git push origin feature/new-feature

# Create pull request for code review
```

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName -p src/OrderManagement.Infrastructure -s src/OrderManagement.API

# Update database
dotnet ef database update -s src/OrderManagement.API

# Remove last migration
dotnet ef migrations remove -p src/OrderManagement.Infrastructure -s src/OrderManagement.API
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Run all tests to ensure they pass
6. Submit a pull request

## 📊 Monitoring and Maintenance

### Log Locations

* **Console**: Real-time application logs
* **Files**: `/logs/log-{date}.txt`
* **Docker**: `docker-compose logs -f ordermanagement-api`

### Database Maintenance

```bash
# Backup database
sqlcmd -S localhost -Q "BACKUP DATABASE OrderManagementDb TO DISK = 'C:\Backup\OrderManagement.bak'"

# Check database health
curl http://localhost:5000/health
```

## 🔮 Future Enhancements

### Phase 1

* [ ] User authentication and authorization
* [ ] Email notifications with templates
* [ ] Payment integration
* [ ] Advanced reporting

### Phase 2

* [ ] Event sourcing for audit trail
* [ ] Microservices decomposition
* [ ] Message queues (RabbitMQ/Azure Service Bus)
* [ ] API rate limiting

### Phase 3

* [ ] Multi-tenant support
* [ ] Advanced analytics dashboard
* [ ] Machine learning for demand forecasting
* [ ] Mobile app API support

## 📞 Support

For questions or issues:

1. Check the documentation
2. Review existing GitHub issues
3. Create a new issue with detailed description
4. Contact the development team

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Built with ❤️ using Clean Architecture and Domain-Driven Design principles**
