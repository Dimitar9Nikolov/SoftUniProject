# SoftUniProject - Delivery Management System

## Overview
AnyThingDelivery is a web-based delivery management
platform designed to give customers the allows customers to order whatever 
they want from wherever they want without 
restrictions from stores and restaurants. 
It also provides an easy why for people to earn some extra income by fuffiling deliveries.
The application provides two distinct interfaces: one for customers to place and track 
orders, and another for delivery personnel to find available deliveries,
manage their active tasks, and track their earnings.

## Key Features
- **Customer Portal**: Create delivery orders with pickup and delivery addresses.
- **Deliveryman Portal**: 
  - Browse available pending orders.
  - Accept orders for delivery.
  - Update order status (e.g., Picked Up, In Transit, Delivered).
  - Track total earnings from completed deliveries.
- **Order Tracking**: Real-time status updates for both customers and delivery staff.
- **User Management**: Secure authentication and role-based access control.

## Technologies Used
- **Backend**: ASP.NET Core MVC (Targeting .NET 10.0)
- **Database**: SQL Server with Entity Framework Core
- **Identity**: ASP.NET Core Identity for authentication and authorization
- **Frontend**: Razor Views, HTML5, CSS3, JavaScript, Bootstrap
- **Tools**: EF Core Migrations, Identity UI

## Getting Started

### Prerequisites
- .NET 10.0 SDK
- SQL Server (LocalDB or Express)
- Visual Studio 2022 / JetBrains Rider / VS Code

### Installation
1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   ```
2. **Configure the database**:
   Update the connection string in `appsettings.json` if necessary.
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SoftUniProject;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```
3. **Run Migrations**:
   ```bash
   dotnet ef database update
   ```
4. **Run the Application**:
   ```bash
   dotnet run
   ```

## Register and Login 
    
The application doesn't require a real email or phone number at the current version.
Registration and login can be done with any random email and phone number that matches the
pattern of an email and phone.

### Test Accounts (Development Mode)
When running in Development mode, the application automatically seeds the following test accounts with example orders:

#### Customer Accounts
- **Email**: `customer1@test.com` | **Password**: `Test123!`
- **Email**: `customer2@test.com` | **Password**: `Test123!`
- **Email**: `customer3@test.com` | **Password**: `Test123!`

#### Delivery Man Accounts
- **Email**: `delivery1@test.com` | **Password**: `Test123!`
- **Email**: `delivery2@test.com` | **Password**: `Test123!`
