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

## Project Structure

```
E-CommAPI/
??? Controllers/
?   ??? AuthController.cs          # User authentication endpoints
?   ??? ProductsController.cs      # Product management endpoints
?   ??? OrdersController.cs        # Order management endpoints
??? Models/
?   ??? User.cs                    # User entity
?   ??? Product.cs                 # Product entity
?   ??? Order.cs                   # Order entity
?   ??? OrderItem.cs               # Order line items
??? Data/
?   ??? ECommerceContext.cs        # Entity Framework DbContext
??? Services/
?   ??? IApiKeyService.cs          # API Key authentication service
?   ??? IProductService.cs         # Product business logic
?   ??? IOrderService.cs           # Order business logic
?   ??? ApiKeyMiddleware.cs        # API Key validation middleware
??? Program.cs                     # Application startup configuration
??? appsettings.json              # Configuration file
```

## Getting Started

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

2. **Configure the database connection**
   
   Update `appsettings.json` with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=ECommerceDB;Trusted_Connection=true;"
     }
   }
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

   The API will be available at: `https://localhost:7000`

## API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Login and receive API key | No |
| POST | `/api/auth/revoke` | Revoke API key | Yes |
| POST | `/api/auth/reset-key` | Reset API key | Yes |

### Products (`/api/products`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| GET | `/api/products` | Get all products (with search/filter) | No |
| GET | `/api/products/{id}` | Get product by ID | No |
| POST | `/api/products` | Create new product | Yes (Admin) |
| PUT | `/api/products/{id}` | Update product | Yes (Admin) |
| DELETE | `/api/products/{id}` | Delete product | Yes (Admin) |

### Orders (`/api/orders`)

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---|
| GET | `/api/orders` | Get user's orders | Yes |
| GET | `/api/orders/{id}` | Get order details | Yes |
| POST | `/api/orders` | Create new order | Yes |
| PUT | `/api/orders/{id}` | Update order | Yes |
| DELETE | `/api/orders/{id}` | Delete order | Yes |

## Authentication

The API uses API Key-based authentication via the `X-API-Key` header.

### Using the API Key

Include the API key in the request header:
```http
X-API-Key: your-api-key-here
```

### Example Request

```bash
curl -X GET "https://localhost:7000/api/products" \
  -H "X-API-Key: your-api-key-here"
```

### Getting an API Key

1. **Register a new user**:
   ```bash
   curl -X POST "https://localhost:7000/api/auth/register" \
     -H "Content-Type: application/json" \
     -d '{
       "username": "newuser",
       "email": "user@example.com"
     }'
   ```

2. **Login**:
   ```bash
   curl -X POST "https://localhost:7000/api/auth/login" \
     -H "Content-Type: application/json" \
     -d '{
       "username": "newuser",
       "email": "user@example.com"
     }'
   ```

   The response will contain your API key.

## Sample Data

The database is initialized with sample data:

**Users:**
- Admin: `admin` / `admin@store.com`
- Customer: `customer1` / `customer1@email.com`

**Products:**
- Gaming Laptop ($1,299.99)
- Wireless Headphones ($199.99)
- Smart Watch ($299.99)

## Swagger Documentation

Once the application is running, access the interactive API documentation at:

```
https://localhost:7000/swagger/index.html
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ECommerceDB;Trusted_Connection=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Database Schema

### Users Table
- Id (PK)
- Username
- Email
- Role (Admin/Customer)
- ApiKey
- ApiKeyExpiry
- CreatedAt
- IsActive

### Products Table
- Id (PK)
- Name
- Description
- Price
- StockQuantity
- Category
- CreatedAt
- UpdatedAt

### Orders Table
- Id (PK)
- UserId (FK)
- OrderDate
- TotalAmount
- Status
- CreatedAt

### OrderItems Table
- Id (PK)
- OrderId (FK)
- ProductId (FK)
- Quantity
- Price

## Error Handling

The API returns standard HTTP status codes:

- **200**: Success
- **201**: Created
- **400**: Bad Request
- **401**: Unauthorized
- **403**: Forbidden
- **404**: Not Found
- **500**: Internal Server Error

Error responses include a descriptive message:
```json
{
  "error": "Username or email already exists"
}
```

## Security Considerations

- API Keys are required for most endpoints
- API Keys expire after 1 year
- Only Admin users can create/update/delete products
- HTTPS is enforced in production
- Request validation prevents invalid data submission
- SQL injection is prevented through parameterized queries

## Development

### Running Tests

```bash
dotnet test
```

### Building for Production

```bash
dotnet publish -c Release -o ./publish
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues and questions, please open an issue on the [GitHub repository](https://github.com/Sunz403/E-CommerceAPI/issues).

## Future Enhancements

- Payment gateway integration
- Email notifications
- Order tracking and shipping
- Product reviews and ratings
- Wishlist functionality
- Cart management
- User profile management
- Advanced analytics and reporting
