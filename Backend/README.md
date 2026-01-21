InternHub Project

# Tech Stack (Backend)

ASP.NET Core Web API

Entity Framework Core + Npgsql.EntityFrameworkCore.PostgreSQL

JWT Authentication (with role-based authorization)

AutoMapper (for mapping entities ↔ DTOs)

FluentValidation (for validating DTOs)

SignalR (for real-time notifications)

# Create Backend Project

dotnet new solution -n InternMS
mkdir InternMS.Api
mkdir InternMS.Domain
mkdir InternMS.Infrastructure

dotnet new webapi -n InternMS.Api
dotnet new classlib -n InternMS.Domain
dotnet new classlib -n InternMS.Infrastructure
dotnet new solution -n InternMS

dotnet sln InternMS.slnx add InternMS.Api/InternMS.Api.csproj
dotnet sln InternMS.slnx add InternMS.Domain/InternMS.Domain.csproj
dotnet sln InternMS.slnx add InternMS.Infrastructure/InternMS.Infrastructure.csproj

# Add Project References

dotnet add InternMS.Api/InternMS.Api.csproj reference InternMS.Domain/InternMS.Domain.csproj
dotnet add InternMS.Api/InternMS.Api.csproj reference InternMS.Infrastructure/InternMS.Infrastructure.csproj

# Install Required NuGet Packages

dotnet add InternMS.Api package Microsoft.EntityFrameworkCore
dotnet add InternMS.Api package Microsoft.EntityFrameworkCore.Design
dotnet add InternMS.Api package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add InternMS.Api package Swashbuckle.AspNetCore
dotnet add InternMS.Api package Microsoft.AspNetCore.Authentication.JwtBearer

# add EFCore to the Infrastructure project

dotnet add InternMS.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add InternMS.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL

# some additional dependency installation

dotnet add InternMS.Api package BCrypt.Net-Next

# Initialize Backend

Create ASP.NET Core Web API project.
Add EF Core & Npgsql.
Configure AppDbContext and connection string to Postgres.
Add migration and update database.
Implement entities & DbContext.
Implement JWT auth & role-based authorization.
Implement controllers for auth, users, projects, notifications.
I am using "PostgreSQL" Database for this application, Because it's more powerful, more flexible, and more standards-compliant than MySQL. 


# Core data model & database
Create domain entities in InternMS.Domain
Add packages:
1. dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
2. dotnet add package Microsoft.EntityFrameworkCore.Design
Add AppDbContext in Infrastructure project and configure PostgreSQL connection string.
Create migration:
1. dotnet ef migrations add InitialCreate --project InternMS.Api --startup-project InternMS.Api
2. dotnet ef database update --project InternMS.Api --startup-project InternMS.Api


# Run the backend
cd InternMS.Api
dotnet restore
dotnet clean
dotnet build
dotnet run