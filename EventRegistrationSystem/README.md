# Event Registration System

A complete event registration and management platform built with **Clean Architecture**. This project consists of a backend developed with **ASP.NET Core (C#)** and a frontend developed with **React (Vite, TypeScript)**.

## Project Overview

The system allows three types of users to interact:
1. **User**: Can browse events, register, view their own registrations, and cancel them.
2. **Organizer**: Can manage their own events and view participants.
3. **Admin**: Has full access to manage all users, roles, and events through a dedicated Admin Panel.

## Architecture

This project strictly follows **Clean Architecture** principles to separate concerns and ensure maintainability:

```text
Domain Layer
    ↓
Application Layer
    ↓
Infrastructure & Persistence Layer
    ↓
API Layer
```

- **Domain**: Contains Enterprise logic (Entities, Enums, Value Objects) and constants (e.g., Permissions). No external dependencies.
- **Application**: Contains Business logic (CQRS using MediatR, Validation using FluentValidation). Depends only on the Domain layer. Uses `IAppDbContext` directly with EF Core to avoid the unnecessary repository anti-pattern.
- **Infrastructure & Persistence**: Contains Data Access (Entity Framework Core, PostgreSQL) and external services (JWT Authentication).
- **API (Presentation)**: Exposes RESTful endpoints, handles HTTP requests, and sets up Dependency Injection. Thin controllers that only delegate to MediatR.

## Tech Stack

### Backend
- **Framework**: .NET 10 (ASP.NET Core Web API)
- **Architecture**: Clean Architecture, CQRS (MediatR)
- **Database**: PostgreSQL (Entity Framework Core)
- **Authentication**: JWT Bearer Tokens with Custom Permission-based Authorization
- **Validation**: FluentValidation

### Frontend
- **Framework**: React 18 (Vite, TypeScript)
- **Routing**: React Router DOM
- **Styling**: Vanilla CSS (Custom Design System, Responsive)
- **HTTP Client**: Axios (with Interceptors for JWT attachment)
- **Icons**: Lucide React

## Features

- **User Authentication**: Secure Login & Registration with JWT.
- **Role-Based Access Control**: Admin, Organizer, and User roles.
- **Dynamic Permissions**: Fine-grained permissions (e.g., `Events.Create`, `Registrations.CancelOwn`).
- **Event Management**: Create, Read, Update, and Delete events (Organizers and Admins).
- **Event Registration**: Users can register for upcoming events until capacity is reached.
- **My Registrations**: Users can view and cancel their registrations.
- **Admin Dashboard**: Specialized interface for Admins to manage users and change roles dynamically.
- **Responsive UI**: A modern, beautiful, and dynamic UI that adapts to any screen size.

## Project Structure

```
EventRegistrationSystem/
├── backend/
│   ├── src/
│   │   ├── Core/
│   │   │   ├── Domain/
│   │   │   └── Application/
│   │   ├── Infrastructure/
│   │   │   ├── EventRegistrationSystem.Infrastructure/
│   │   │   └── EventRegistrationSystem.Persistence/
│   │   └── API/
│   ├── EventRegistrationSystem.Application.UnitTests/
│   └── EventRegistrationSystem.API.IntegrationTests/
├── frontend/
│   ├── src/
│   │   ├── pages/
│   │   ├── components/
│   │   ├── assets/
│   │   └── api.ts
│   ├── package.json
│   └── vite.config.ts
├── docker-compose.yml
└── README.md
```

## Environment Variables

### Docker (`.env`)
To ensure security, sensitive credentials are not hardcoded in the codebase.
Copy the `.env.example` file to a new `.env` file in the root directory:

```bash
cp .env.example .env
```

Then edit the `.env` file to customize your passwords and secrets.

## How to Run

### Method 1: Using Docker Compose (Recommended)
You can run the entire stack (PostgreSQL, Backend, Frontend) with a single command:
```bash
docker-compose up --build
```
- Frontend will be available at `http://localhost:5173`
- Backend API & Swagger will be available at `http://localhost:5002/swagger`

### Method 2: Local Development
**Prerequisites:** .NET 10 SDK, Node.js, PostgreSQL.

1. **Database Setup**
   Ensure PostgreSQL is running locally on port `5432` with username `postgres` and password `password`.
   
2. **Run Backend**
   ```bash
   cd backend/src/API
   dotnet restore
   dotnet run
   ```
   *Migrations and seed data will be applied automatically on startup.*

3. **Run Frontend**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```

## Authentication & Initial Users

The database automatically seeds the roles (`Admin`, `Organizer`, `User`) and an initial Admin account.

**Default Admin Account:**
- **Email**: `admin@example.com`
- **Password**: `Admin123!`

You can use this account to login and test the Admin Panel.

## API Endpoints

A full Swagger documentation is available at `/swagger/index.html` when running the API.
Key Endpoints:
- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Authenticate and get JWT
- `GET /api/events` - Get all upcoming events
- `POST /api/events` - Create a new event (Organizer/Admin)
- `POST /api/events/{id}/registrations` - Register for an event
- `DELETE /api/registrations/{id}` - Cancel an event registration

## Testing

The backend includes a comprehensive suite of automated tests:
- **Unit Tests**: Utilizing xUnit and Moq to test the Application layer (MediatR Handlers, Validation).
- **Integration Tests**: End-to-End API tests verifying database interactions and HTTP responses.

Run tests using:
```bash
cd backend
dotnet test
```
