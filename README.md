# ClinicApp API

A RESTful API for a multi-tenant clinic management platform built with **ASP.NET Core 8** and **MySQL**. The platform allows clinic owners to register their clinics, list medical services and products, manage staff, and handle bookings and e-commerce orders. Regular users can browse clinics/services/products, book appointments, and place product orders.

## Features

- **Authentication & Authorization** — JWT-based auth with ASP.NET Core Identity, supporting two roles: `User` and `ClinicOwner`.
- **Clinic Management** — Clinic owners can create and update a single clinic profile, including operating hours, contact info, and tags/categories.
- **Medical Services** — Clinic owners can create, update, and delete services (with pricing, duration, and tags); anyone can browse them.
- **Products** — Clinic owners can manage a product catalog (with stock, pricing, and tags) for an in-app store.
- **Bookings** — Users can view available appointment slots and book medical services; clinic owners manage booking status (confirm, cancel, complete). Staff are auto-assigned based on availability.
- **Orders** — Users can purchase products (cart-style, split automatically across clinics); clinic owners manage order fulfillment status.
- **Staff Management** — Clinic owners can manage staff members and assign them to the medical services they provide.
- **Tags** — A shared tagging system used to categorize clinics, medical services, and products (pre-seeded with common categories).
- **Image Uploads** — Authenticated clinic owners can upload images (JPG/PNG) for clinics, services, and products. Images are stored on **Cloudinary**, with old images automatically deleted on replacement.
- **Email Notifications** — Automated emails via SendGrid for booking and order status updates.
- **Swagger / OpenAPI** — Interactive API documentation with JWT bearer auth support (development environment).

## Tech Stack

- **.NET 8** / ASP.NET Core Web API
- **Entity Framework Core 8** with **Pomelo.EntityFrameworkCore.MySql** (MySQL provider)
- **ASP.NET Core Identity** for user management and roles
- **JWT Bearer Authentication**
- **FluentValidation** for request validation
- **Mapster** for object mapping
- **SendGrid** for transactional emails
- **CloudinaryDotNet** for image hosting/storage
- **Swashbuckle (Swagger)** for API documentation

## Project Structure

```
ClinicApp.API/
├── Controllers/         # API endpoints (Auth, Clinics, MedicalServices, Products, Bookings, Orders, Staff, Tags, Upload)
├── DTOs/                # Request/response data transfer objects, organized by domain
├── Data/                # AppDbContext and database seed logic
├── Helpers/             # Shared utilities (Cloudinary public ID extraction, UTC date converter)
├── Interfaces/          # Service and repository contracts
├── Models/              # EF Core entity models
├── Repositories/        # Data access layer (EF Core implementations)
├── Services/            # Business logic layer
└── Program.cs           # App configuration, DI, middleware pipeline
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MySQL Server 8.x
- A [Cloudinary](https://cloudinary.com/) account (for image uploads)
- A SendGrid account (for email notifications) — optional for local testing if you stub the email service

### Configuration

This project uses **`dotnet user-secrets`** for sensitive configuration in development. Do **not** commit real credentials to `appsettings.json`.

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=ClinicAppDb;user=root;password=YOUR_PASSWORD"
dotnet user-secrets set "Jwt:Key" "your-super-secret-jwt-key-min-32-chars"
dotnet user-secrets set "SendGrid:ApiKey" "your-sendgrid-api-key"
dotnet user-secrets set "Cloudinary:CloudName" "your-cloud-name"
dotnet user-secrets set "Cloudinary:ApiKey" "your-cloudinary-api-key"
dotnet user-secrets set "Cloudinary:ApiSecret" "your-cloudinary-api-secret"
```

The `appsettings.json` file contains the structure/placeholders for:

| Section | Key | Description |
|---|---|---|
| `ConnectionStrings` | `DefaultConnection` | MySQL connection string |
| `Jwt` | `Key`, `Issuer`, `Audience`, `DurationInMinutes` | JWT signing configuration |
| `SendGrid` | `ApiKey`, `FromEmail`, `FromName` | Email notification settings |
| `Cloudinary` | `CloudName`, `ApiKey`, `ApiSecret` | Image storage credentials |

### Database Setup

Apply EF Core migrations to create the database schema (this also seeds roles and tag data):

```bash
dotnet ef database update
```

### Running the API

```bash
dotnet run
```

By default, the API runs on `http://localhost:5085`. In development mode, Swagger UI is available at `/swagger`.

## Authentication

Most write operations require a JWT bearer token. Obtain one via:

- `POST /api/Auth/Register` — Register as `User` or `ClinicOwner`
- `POST /api/Auth/Login` — Returns a JWT containing the user's id, email, and role

Include the token in subsequent requests:

```
Authorization: Bearer <your-token>
```

## API Overview

| Resource | Description |
|---|---|
| `/api/Auth` | Registration and login |
| `/api/Clinics` | Clinic CRUD (owner-only create/update, public read) |
| `/api/MedicalServices` | Medical service CRUD per clinic |
| `/api/Products` | Product catalog CRUD per clinic |
| `/api/Bookings` | Appointment booking, slot availability, status management |
| `/api/Orders` | Product orders, order status management |
| `/api/Staff` | Staff management for clinic owners |
| `/api/Tags` | Browse tags by category (`Clinic`, `MedicalService`, `Product`) |
| `/api/Upload` | Image upload for clinic owners (stored on Cloudinary) |

## Roles

- **User** — Can browse clinics, services, and products; can book appointments and place orders.
- **ClinicOwner** — Can manage a single clinic, its services, products, and staff; manages bookings and orders for their clinic.

## Image Handling

Images for clinics, medical services, and products are uploaded via `POST /api/Upload` (restricted to `ClinicOwner`), which stores the file on Cloudinary under the `clinicapp` folder and returns a `secure_url`. That URL is then submitted as the entity's `ImageUrl` field when creating/updating a clinic, service, or product. When an entity's image is replaced, the previous Cloudinary asset is deleted automatically using its extracted public ID.

The frontend is responsible for displaying a placeholder image (e.g. via placeholder.co) when no `ImageUrl` is set.

## Notes

- Timestamps are stored and returned in UTC; the API converts to local time (Jordan Standard Time) for email notifications.
- Available booking slots are calculated based on clinic operating hours, service duration, and staff availability.
- Tags are pre-seeded and shared across clinics, services, and products for filtering/categorization.

