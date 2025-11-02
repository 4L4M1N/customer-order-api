# Implementation Assumptions
---

## Business Logic

### Customer & Orders
- Customer names (first/last), address, and postal code are **required fields**
- Each customer can have **one active shopping cart** (one-to-one relationship)
- Orders are created **only through cart checkout** (not directly)
- Orders are **immutable** once created (no editing after checkout)
- **Price is captured at time of purchase** 
- Product price changes **don't affect existing orders**
- **No order status workflow** implemented (could add: Pending, Confirmed, Shipped, Complete etc.)

### Shopping Cart Flow
- **Added shopping cart feature**  for realistic e-commerce flow
- Cart is **persistent** 
- Cart is **automatically deleted after checkout**
- Adding same product multiple times **increases quantity**
- After place order cart is removed.

### Validation Rules
- Quantity must be **positive** (> 0)
- Price cannot be **negative**
- Order total is **auto-calculated** 

### Others
- Product and Customer are not deleted from db. If delete operation happen it will be **soft deleted**. 
---

## Technical Architecture

### Database & ORM
- **SQL Server** with Entity Framework Core
- **GUIDs as primary keys** 
- **UTC timezone** for all dates

### CQRS Pattern
- **Single database** for commands and queries (simplified CQRS)
- **No mediator library** (direct handler invocation)
- Commands **return IDs** for newly created entities
- Queries return **DTOs** (not domain entities)

### Repository & Unit of Work
- **Generic repository** with specialized implementations where needed
- **Async methods** for non-blocking I/O
- Unit of Work **manages transactions** and coordinates repositories


### API Design
- Standard **REST conventions** (GET, POST, PUT, DELETE)
- **JSON** for request/response
- Standard **HTTP status codes** (200, 201, 204, 400, 404)
- **Domain exceptions** translated to HTTP status codes in controllers
- **No pagination** for list endpoints

---

## Security & Authentication

- **No authentication/authorization** implemented 
- API is **open** 
- **HTTPS enforced** via middleware
- **Input validation** in domain layer only

---

## Testing

- **Unit tests** for domain logic and command handlers
- **Mocked dependencies** using Moq 
- **NUnit** as test framework 
- Focus on **happy path and validation scenarios**

---

## Not Implemented (Out of Scope)

These were considered but not implemented

### Features
- Order status workflow
- Inventory/stock management
- Multi-currency support
- Customer time zone support. 
- Pagination for list

### Technical
- API versioning, health checks
- Global exception handling middleware
- Structured logging (Serilog)

---
