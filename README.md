# E-Commerce API

A RESTful API for managing an e-commerce platform with user authentication, product management, and order processing. Built with ASP.NET Core 8 and Entity Framework Core.

## Features

- **User Authentication & Authorization**
  - User registration and login
  - API Key-based authentication
  - Role-based access control (Admin, Customer)
  - API Key generation, reset, and revocation

- **Product Management**
  - Browse products with search and category filtering
  - Create, update, and delete products (Admin only)
  - Product inventory management
  - Categorized product listings

- **Order Management**
  - Create orders with multiple items
  - View order history
  - Order item management
  - Real-time inventory updates

- **API Features**
  - RESTful endpoints with JSON and XML support
  - Swagger/OpenAPI documentation
  - HTTPS security with automatic redirection
  - Request/response validation
  - Comprehensive error handling

## Technology Stack

- **Framework**: ASP.NET Core 8
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Authentication**: API Key (X-API-Key header)
- **Documentation**: Swagger/OpenAPI
- **Serialization**: JSON & XML support

### Prerequisites

- .NET 8 SDK
- SQL Server (local or remote)
- Visual Studio 2022 or Visual Studio Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Sunz403/E-CommerceAPI.git
   cd E-CommAPI
   ```
## Swagger Documentation

Once the application is running, access the interactive API documentation at:

```
https://localhost:7000/swagger/index.html
```

## Security Considerations

- API Keys are required for most endpoints
- API Keys expire after 1 year
- Only Admin users can create/update/delete products
- HTTPS is enforced in production
- Request validation prevents invalid data submission
- SQL injection is prevented through parameterized queries

## License

This project is licensed under the MIT License - see the LICENSE file for details.


