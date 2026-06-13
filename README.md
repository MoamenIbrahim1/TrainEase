# TrainEase


## Project Overview

**TrainEase** is a train reservation backend API built with **ASP.NET Core Web API**, **.NET 8**, **SQL Server**, and **Entity Framework Core**.

The system supports the main workflows of a railway booking platform, including passenger authentication, journey search, seat availability, multi-passenger booking, online payment initiation, payment webhook handling, offers, train delay reporting, notifications, dashboard statistics, and train tracking.

This project was developed as my graduation project and demonstrates my backend development skills in building real-world, layered, maintainable, and business-oriented APIs.

* **Live Demo:** https://train-ease.netlify.app/
* **Backend API Documentation:** Swagger UI is enabled from the API host.
* **Role:** .NET Backend Developer
* **GitHub:** https://github.com/3OmarReda
* **LinkedIn:** https://www.linkedin.com/in/omarreda7365
* **Email:** [omarredaelkholy@gmail.com](mailto:omarredaelkholy@gmail.com)

---

## Key Features

* User authentication and authorization using ASP.NET Core Identity and JWT.
* Passenger registration, email confirmation, login, profile update, forgot password, and reset password.
* Role-based user model with `Admin`, `Passenger`, and `StationMaster` roles.
* Train, carriage, seat, station, journey, and schedule management.
* Journey search by source station, destination station, and travel date.
* Seat availability checking for selected journeys.
* Multi-passenger booking with validation rules.
* Pending booking expiration after a limited hold time.
* Booking cancellation and refund calculation.
* Paymob payment integration with payment iframe generation.
* Payment webhook handling with HMAC verification.
* Invoice creation after successful payment.
* Offer creation and automatic discount application.
* Train delay reporting by station masters.
* Email notifications for account actions, cancellations, and delay updates.
* Train tracking based on route stations, schedule timing, station coordinates, and delays.
* Dashboard endpoints for trains, passengers, bookings, delays, journeys, and seat statistics.

---

## Tech Stack

| Area               | Technology                                                                    |
| ------------------ | ----------------------------------------------------------------------------- |
| Runtime            | .NET 8                                                                        |
| API Framework      | ASP.NET Core Web API                                                          |
| Authentication     | ASP.NET Core Identity                                                         |
| Authorization      | JWT Bearer Authentication, Role-based Authorization                           |
| Database           | SQL Server                                                                    |
| ORM                | Entity Framework Core 8                                                       |
| EF Features        | Migrations, Fluent API Configurations, Lazy Loading Proxies                   |
| Architecture Style | Clean Architecture / Onion Architecture-inspired Layered Architecture         |
| Mapping            | AutoMapper                                                                    |
| Email              | MailKit, SMTP                                                                 |
| Payments           | Paymob Integration using `HttpClient`                                         |
| API Documentation  | Swagger / Swashbuckle                                                         |
| Configuration      | `appsettings.json`, strongly typed settings classes                           |
| Patterns           | Repository Pattern, Unit of Work, Service Manager, DTOs, Dependency Injection |

---

## Architecture

The project follows a **Clean Architecture / Onion Architecture-inspired layered structure**.

The solution is separated into multiple layers, where each layer has a clear responsibility. Controllers expose the API endpoints, application services contain the business logic, the domain layer contains the core entities and contracts, and the persistence layer handles database access through Entity Framework Core.

```text
Client / Frontend
      |
      v
TrainEase.APIs
API host, Program.cs, Swagger, CORS, authentication pipeline
      |
      v
TrainEase.APIs.Controllers
REST controllers and API response models
      |
      v
TrainEase.Core.Application.Abstrations
Service contracts and request/response DTOs
      |
      v
TrainEase.Core.Application
Business services, mapping, exceptions, email/payment settings
      |
      v
TrainEase.Core.Domain
Entities, enums, base entity classes, repository/unit-of-work contracts
      |
      v
TrainEase.Infrastructure.Presistance
EF Core DbContext, migrations, entity configurations, repositories, Unit of Work
```

### Main Architectural Components

* **API Host:** Configures dependency injection, authentication, authorization, CORS, Swagger, and middleware pipeline.
* **Controllers:** Expose RESTful endpoints and delegate business logic to application services.
* **Application Services:** Contain business workflows such as authentication, booking, payment, journey search, offers, tracking, and delay reporting.
* **Domain Layer:** Contains the main business entities, enums, shared base classes, and contracts.
* **Persistence Layer:** Handles SQL Server database access using EF Core, repositories, migrations, configurations, and Unit of Work.
* **DTOs:** Separate API contracts from internal domain entities.
* **Repository Pattern:** Encapsulates data access logic.
* **Unit of Work:** Coordinates database operations and transaction completion.
* **Service Manager:** Provides access to application services from controllers.

---

## Folder Structure

```text
TrainEase/
├── TrainEase.APIs/
│   ├── Program.cs
│   ├── Extensions/
│   │   └── IdentityExtensions.cs
│   ├── Middlewares/
│   │   └── ExceptionHandlerMiddleware.cs
│   ├── appsettings.json
│   └── Properties/
│
├── TrainEase.APIs.Controllers/
│   ├── Controllers/
│   │   ├── Account/
│   │   ├── Admin/
│   │   ├── Booking/
│   │   ├── Dashboard/
│   │   ├── Delays/
│   │   ├── Journey/
│   │   ├── Offers/
│   │   ├── Payment/
│   │   ├── Stations/
│   │   ├── Tracking/
│   │   └── Train/
│   └── Errors/
│
├── TrainEase.Core.Application.Abstrations/
│   ├── Model/
│   │   ├── Auth/
│   │   ├── Booking/
│   │   ├── Carriage/
│   │   ├── Dashboard/
│   │   ├── Delays/
│   │   ├── Journey/
│   │   ├── Offers/
│   │   ├── Payment/
│   │   ├── Stations/
│   │   ├── Tracking/
│   │   └── Train/
│   └── Services/
│
├── TrainEase.Core.Application/
│   ├── Exceptions/
│   ├── Mapping/
│   ├── Services/
│   │   ├── Admin/
│   │   ├── Auth/
│   │   ├── Booking/
│   │   ├── Dashboard/
│   │   ├── Delays/
│   │   ├── Email/
│   │   ├── Journey/
│   │   ├── Offers/
│   │   ├── Payment/
│   │   ├── Station/
│   │   ├── Tracking/
│   │   └── Trains/
│   └── Settings/
│
├── TrainEase.Core.Domain/
│   ├── Common/
│   ├── Contracts/
│   └── Entities/
│       └── Enums/
│
├── TrainEase.Infrastructure.Presistance/
│   ├── _Identity/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/
│   ├── Migrations/
│   ├── Repostories/
│   ├── Seeding/
│   └── UnitOfWork/
│
├── TrainEase.Infrastructure/
├── TrainEase.slnx
└── README.md
```

---

## API Modules

### Account Module

Handles authentication, user registration, email confirmation, profile management, and password reset flows.

| Method | Endpoint                       | Description                         |
| ------ | ------------------------------ | ----------------------------------- |
| `POST` | `/api/Account/register`        | Register a new passenger            |
| `POST` | `/api/Account/Login`           | Login and generate JWT token        |
| `GET`  | `/api/Account/confirm-email`   | Confirm user email                  |
| `GET`  | `/api/Account/My-profile`      | Retrieve authenticated user profile |
| `PUT`  | `/api/Account/Update-profile`  | Update authenticated user profile   |
| `POST` | `/api/Account/forgot-password` | Send password reset email           |
| `POST` | `/api/Account/reset-password`  | Reset user password                 |

---

### Admin Module

Handles station master assignment and station master management.

| Method   | Endpoint                                           | Description                                      |
| -------- | -------------------------------------------------- | ------------------------------------------------ |
| `POST`   | `/api/Admin/AssignStationMaster`                   | Assign a station master to a station             |
| `DELETE` | `/api/Admin/RemoveStationMaster/{stationMasterId}` | Remove station master using soft-delete behavior |
| `GET`    | `/api/Admin/GetAllStationMasters`                  | Retrieve all station masters                     |

---

### Train Module

Handles train creation, editing, deletion, and carriage class statistics.

| Method   | Endpoint                               | Description                                       |
| -------- | -------------------------------------- | ------------------------------------------------- |
| `GET`    | `/api/Train/GetTrains`                 | Retrieve all trains                               |
| `POST`   | `/api/Train/AddTrain`                  | Create a train with carriages and generated seats |
| `PUT`    | `/api/Train/EditTrain/{id}`            | Edit train metadata and carriage counts           |
| `DELETE` | `/api/Train/DeleteTrain`               | Delete train                                      |
| `GET`    | `/api/Train/GetTrainClasses/{trainId}` | Retrieve train carriage class statistics          |

---

### Journey Module

Handles journey creation, journey search, and journey deletion.

| Method   | Endpoint                                 | Description                                             |
| -------- | ---------------------------------------- | ------------------------------------------------------- |
| `GET`    | `/api/Journey/search`                    | Search future journeys by source, destination, and date |
| `POST`   | `/api/Journey/CreateJourney`             | Create a journey with schedules and stations            |
| `DELETE` | `/api/Journey/DeleteJourney/{journeyId}` | Delete journey                                          |

---

### Booking Module

Handles seat availability, booking creation, cancellation, and booking retrieval.

| Method | Endpoint                                   | Description                                       |
| ------ | ------------------------------------------ | ------------------------------------------------- |
| `GET`  | `/api/Bookings/journeys/{journeyId}/seats` | Retrieve seat availability for a journey          |
| `POST` | `/api/Bookings`                            | Create a booking                                  |
| `GET`  | `/api/Bookings/AllBookingsByUser`          | Retrieve bookings for the authenticated passenger |
| `GET`  | `/api/Bookings/{bookingId}`                | Retrieve booking by ID                            |
| `PUT`  | `/api/Bookings/cancel/{bookingId}`         | Cancel a pending or confirmed booking             |

---

### Payment Module

Handles Paymob payment initiation and webhook processing.

| Method | Endpoint                            | Description                         |
| ------ | ----------------------------------- | ----------------------------------- |
| `POST` | `/api/Payment/initiate/{bookingId}` | Initiate card payment for a booking |
| `POST` | `/api/Payment/webhook`              | Handle Paymob payment webhook       |

---

### Offers Module

Handles creation and retrieval of active offers.

| Method | Endpoint            | Description                          |
| ------ | ------------------- | ------------------------------------ |
| `POST` | `/api/Offer/create` | Create an offer for a future journey |
| `GET`  | `/api/Offer/active` | Retrieve active offers               |

---

### Delay Reporting Module

Handles train delay reporting and delay-related notifications.

| Method | Endpoint                               | Description                                                  |
| ------ | -------------------------------------- | ------------------------------------------------------------ |
| `POST` | `/api/TrainDelay/report`               | Report a train delay                                         |
| `GET`  | `/api/TrainDelay/DelaysInMyStation`    | Retrieve delays for the station master’s station             |
| `GET`  | `/api/TrainDelay/my-station/schedules` | Retrieve upcoming schedules for the station master’s station |

---

### Tracking Module

Handles train tracking and journey progress calculation.

| Method | Endpoint                     | Description                                     |
| ------ | ---------------------------- | ----------------------------------------------- |
| `GET`  | `/api/tracking/{scheduleId}` | Retrieve train tracking information by schedule |

---

### Stations Module

Handles station retrieval.

| Method | Endpoint                      | Description           |
| ------ | ----------------------------- | --------------------- |
| `GET`  | `/api/Station/GetAllStations` | Retrieve all stations |

---

### Dashboard Module

Provides dashboard statistics and admin-oriented data.

| Method | Endpoint                                  | Description                            |
| ------ | ----------------------------------------- | -------------------------------------- |
| `GET`  | `/api/Dashboard/GetTotalTrains`           | Retrieve total trains count            |
| `GET`  | `/api/Dashboard/GetTotalActivePassengers` | Retrieve total active passengers count |
| `GET`  | `/api/Dashboard/GetTodaysBookings`        | Retrieve today’s bookings count        |
| `GET`  | `/api/Dashboard/GetAllTrainSeatStats`     | Retrieve train seat statistics         |
| `GET`  | `/api/Dashboard/GetAllDelays`             | Retrieve all delay reports             |
| `GET`  | `/api/Dashboard/AllBookings`              | Retrieve all bookings                  |
| `GET`  | `/api/Dashboard/AllJourneys`              | Retrieve all upcoming journeys         |

---

## Core Business Workflows

### Authentication Workflow

The authentication module uses ASP.NET Core Identity and JWT Bearer Authentication.

Main supported flows:

* Passenger registration.
* Email confirmation.
* Login and JWT token generation.
* Authenticated profile retrieval.
* Profile update.
* Forgot password.
* Reset password.
* Role-based user model.

---

### Booking Workflow

The booking workflow supports seat reservation for one or more passengers.

Main validations include:

* Checking that selected seats belong to the selected train.
* Preventing duplicate selected seats in the same booking.
* Preventing duplicate national IDs in the same booking.
* Preventing reservation of already booked seats.
* Calculating total booking price.
* Applying active offer discounts when available.
* Holding pending bookings for a limited time before expiration.
* Allowing cancellation for pending or confirmed bookings.
* Applying refund rules based on cancellation time before departure.

---

### Payment Workflow

The payment module integrates with Paymob.

Main steps:

1. The passenger creates a booking.
2. The system initiates a payment request through Paymob.
3. Paymob returns a payment key and iframe payment URL.
4. The passenger completes the payment.
5. Paymob sends a webhook to the backend.
6. The backend verifies the webhook HMAC signature.
7. The backend validates the paid amount against the booking total.
8. The booking is marked as confirmed after successful payment.
9. An invoice is generated for the confirmed payment.

---

### Delay Reporting Workflow

Station masters can report train delays for schedules assigned to their station.

The system then:

* Stores the delay report.
* Calculates affected schedule information.
* Sends email notifications to confirmed booking owners and passengers.
* Creates internal notification records for booking owners.
* Allows admins to view all delay reports.
* Allows station masters to view delays related to their station.

---

### Train Tracking Workflow

The tracking module calculates train movement status based on:

* Schedule departure and arrival time.
* Route stations.
* Station coordinates.
* Delay records.
* Current time.

The returned tracking data includes:

* Current segment between stations.
* Progress percentage.
* Estimated remaining minutes.
* Current coordinates.
* Train status such as `NotStarted`, `Moving`, `Delayed`, and `Completed`.

---

## Database Design Overview

The domain model includes the following main entities:

* `User`
* `Passenger`
* `Station`
* `Train`
* `Carriage`
* `Seat`
* `Route`
* `RouteStation`
* `Schedule`
* `Booking`
* `Ticket`
* `Payment`
* `Invoice`
* `Refund`
* `Notification`
* `Offer`
* `TrainDelay`
* `TrainTracking`

The database is managed using **Entity Framework Core Migrations** and configured using Fluent API configuration classes.

---

## Getting Started

### Prerequisites

Make sure the following tools are installed:

* .NET 8 SDK
* SQL Server
* Visual Studio 2022 or Visual Studio Code
* Entity Framework Core CLI tool compatible with EF Core 8
* SMTP email account or app password
* Paymob test or production credentials

---

## Installation

Clone the repository:

```bash
git clone https://github.com/3OmarReda/TrainEase.git
cd TrainEase
```

Restore dependencies:

```bash
dotnet restore TrainEase.APIs/TrainEase.APIs.csproj
```

---

## Configuration

Create or update `TrainEase.APIs/appsettings.Development.json` with your local configuration.

> Do not commit real secrets, passwords, payment keys, email passwords, or production connection strings.

Example configuration shape:

```json
{
  "ConnectionStrings": {
    "IdentityContext": "Server=YOUR_SERVER;Database=TrainEaseDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "jwtSettings": {
    "Key": "YOUR_LONG_SECURE_JWT_KEY",
    "Audience": "TrainEaseUsers",
    "Issuer": "TrainEaseIdentity",
    "DurationInMinutes": 50
  },
  "Paymob": {
    "ApiKey": "YOUR_PAYMOB_API_KEY",
    "IntegrationId": 0,
    "IframeId": "YOUR_IFRAME_ID",
    "HmacSecret": "YOUR_PAYMOB_HMAC_SECRET",
    "SecretKey": "YOUR_PAYMOB_SECRET_KEY"
  },
  "AppSettings": {
    "BaseUrl": "https://localhost:7039",
    "FrontEndUrl": "http://127.0.0.1:5500"
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderName": "TrainEase",
    "SenderEmail": "your-email@gmail.com",
    "Username": "your-email@gmail.com",
    "Password": "YOUR_EMAIL_APP_PASSWORD"
  }
}
```

---

## Database Setup

Apply EF Core migrations:

```bash
dotnet ef database update \
  --project TrainEase.Infrastructure.Presistance/TrainEase.Infrastructure.Presistance.csproj \
  --startup-project TrainEase.APIs/TrainEase.APIs.csproj
```

The API startup runs database migration automatically and seeds Identity roles through `IdentitySeeder.SeedRolesAsync`.

---

## Run Instructions

Run the API:

```bash
dotnet run --project TrainEase.APIs/TrainEase.APIs.csproj
```

Default local URLs from `launchSettings.json`:

```text
HTTP:  http://localhost:5135
HTTPS: https://localhost:7039
```

Swagger UI is enabled and configured as the default route.

```text
https://localhost:7039/
http://localhost:5135/
```

---

## CORS

The API currently allows the following frontend origins:

```text
http://127.0.0.1:5500
http://192.168.47.1:5000
https://train-ease.netlify.app
```

---

## Screenshots

Screenshots will be added soon.

Suggested screenshots:

* Swagger UI home page.
* Login and registration flow.
* Journey search results.
* Seat selection page.
* Booking details.
* Payment iframe flow.
* Admin dashboard.
* Train delay notification flow.
* Train tracking status.

```md
![Swagger UI](screenshots/swagger.png)
![Journey Search](screenshots/journey-search.png)
![Seat Selection](screenshots/seat-selection.png)
![Dashboard](screenshots/dashboard.png)
```

---

## My Role

As the **.NET Backend Developer** on TrainEase, my responsibilities included building and maintaining the backend API, designing the layered project structure, implementing the main business services, configuring EF Core persistence, handling authentication and authorization workflows, integrating payment and email services, and exposing backend functionality through documented REST endpoints.

This project demonstrates my ability to work with real backend requirements such as booking rules, seat availability, payment confirmation, refund logic, role-based workflows, dashboard statistics, notifications, and relational database design.

---

## What I Learned

* Designing a layered ASP.NET Core backend using Clean Architecture principles.
* Modeling complex business relationships with Entity Framework Core.
* Implementing authentication and role-based authorization using ASP.NET Core Identity and JWT.
* Building booking workflows with validation, pending states, expiration logic, cancellation, and refunds.
* Integrating third-party payment providers and validating payment webhooks.
* Sending transactional emails for account confirmation, password reset, cancellations, and delay notifications.
* Using Repository and Unit of Work patterns to organize persistence logic.
* Building dashboard-oriented endpoints for operational statistics.
* Calculating train tracking progress using station coordinates, route segments, schedule timing, and delay data.

---

## Status

TrainEase is a graduation project backend API with the main business modules implemented.

The project is suitable for demonstrating backend engineering skills in:

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT Authentication
* Role-based Authorization
* Clean Architecture-inspired design
* Payment integration
* Email notifications
* Real-world booking workflow design

---

## Future Improvements

* Move all sensitive configuration values to environment variables or user secrets.
* Add automated tests for services and controllers.
* Harden authorization for admin-only and management endpoints.
* Add CI/CD pipeline configuration.
* Add request validation using FluentValidation or a similar validation library.
* Add structured logging.
* Improve production-ready exception handling.
* Add API versioning.
* Add Docker support for easier deployment.
* Add more detailed Swagger documentation and examples.

---

## Contact

**Omar Reda El Kholy**
.NET Backend Developer

* **GitHub:** https://github.com/3OmarReda
* **LinkedIn:** https://www.linkedin.com/in/omarreda7365
* **Email:** [omarredaelkholy@gmail.com](mailto:omarredaelkholy@gmail.com)
