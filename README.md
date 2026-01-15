# WireMess

A real-time chat web application API built with ASP.NET Core and SignalR.

## Features

- **User Authentication** - JWT-based authentication with registration, login, and password management
- **Real-time Messaging** - SignalR-powered instant messaging
- **Conversations** - Support for direct (1-on-1) and group chats
- **File Attachments** - Upload and share files in conversations
- **Soft Delete** - Data retention with soft delete functionality

## Tech Stack

- **.NET 9** with C# 13.0
- **ASP.NET Core Web API**
- **Entity Framework Core** - ORM with PostgreSQL
- **SignalR** - Real-time communication
- **JWT** - Authentication & authorization
- **Swagger/OpenAPI** - API documentation

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

## Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/nhatkhoa1124/WireMess.git
   cd WireMess
   ```

2. **Configure the database**
   
   Update `appsettings.json` with your PostgreSQL connection string:
   ```json
   "ConnectionStrings": {
     "WireMessDb": "Host=localhost;Database=wiremess;Username=youruser;Password=yourpassword"
   }
   ```

3. **Configure JWT settings**
   
   Update JWT settings in `appsettings.json`:
   ```json
   "Jwt": {
     "Secret": "your-secret-key-here",
     "Issuer": "WireMessApi",
     "Audience": "WireMessClient"
   }
   ```

4. **Run the application**
   ```bash
   dotnet restore
   dotnet run
   ```

   The API will be available at `https://localhost:7096` (or check console output)

5. **Access Swagger UI**
   
   Navigate to `https://localhost:7096/swagger` for API documentation

## Project Structure

```
WireMess/
├── Controllers/         # API endpoints
├── Services/           # Business logic
├── Repositories/       # Data access layer
├── Models/            
│   ├── Entities/      # Database entities
│   └── DTOs/          # Data transfer objects
├── Data/              # DbContext and configurations
├── Hubs/              # SignalR hubs
└── Utils/             # Helpers and extensions
```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/logout` - Logout user
- `POST /api/auth/change-password` - Change password

### Conversations
- `GET /api/conversations` - Get all conversations
- `GET /api/conversations/{id}` - Get conversation by ID
- `POST /api/conversations` - Create conversation
- `PUT /api/conversations/{id}` - Update group profile
- `DELETE /api/conversations/{id}` - Delete conversation

### Users
- `GET /api/users/{id}` - Get user profile
- `PUT /api/users/{id}` - Update user profile

## Authentication

All protected endpoints require a JWT bearer token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Database Migrations

Migrations are applied automatically in development mode on startup. To manage migrations manually:

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## License

This project is open source and available under the [MIT License](LICENSE).

## Author

**Nhat Khoa**
- GitHub: [@nhatkhoa1124](https://github.com/nhatkhoa1124)
