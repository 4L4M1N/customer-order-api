# Customer-Order Management

A clean .NET solution demonstrating DDD-style domain, CQRS, EF Core, and a REST API for managing customers, products, orders, and shopping carts.

## Tech Stack
- .NET `9.0`
- ASP.NET Core Web API
- EF Core `SqlServer` with code-first migrations
- Swagger (OpenAPI)
- NUnit + Moq for tests

### Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd customer-order-api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update connection string** (if needed)
   
   Edit `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=OrderManagementDb;Trusted_Connection=True;Encrypt=False;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Apply database migrations**

   
   Simply run the application (migrations auto-apply on startup):

5. **Run the application**
   ```bash
   dotnet run --project CustomerOrderManagement.Api
   ```

6. **Access the API**
   - Swagger UI: `http://localhost:5294`
   - API Base URL: `http://localhost:5294/api`

### Running Tests

```bash
# Run all tests
dotnet test
```
