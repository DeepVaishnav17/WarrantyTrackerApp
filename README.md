# Warranty Tracker for Home Appliances

## Project Overview

**Warranty Tracker for Home Appliances** is a startup-style web application designed to help users manage and track warranties for their home appliances. The application provides automated reminders for warranty expiration and allows users to maintain service records for their appliances.

### Problem Statement
Most people lose track of warranty information for their appliances, leading to missed opportunities for free repairs and replacements. Our solution provides a centralized platform to manage warranty information and receive timely reminders.

### Solution
A web-based warranty management system that allows users to:
- Store appliance information with warranty details
- Receive automated reminders before warranty expiration (planned feature)
- Track service history and maintenance records
- Get notifications through dashboard alerts (in development)

## Features Implemented

### Core Features (Minimum Viable Product)

1. **User Authentication & Authorization** ✅
   - User registration and login using ASP.NET Core Identity
   - Role-based access control (User/Admin roles)
   - Secure session management

2. **Appliance Management (CRUD Operations)** ✅
   - Add appliances with purchase and warranty details
   - View, edit, and delete appliance records
   - Upload purchase receipt images (optional)
   - Search and filter appliances by status

3. **Dashboard & Dynamic Views** ✅
   - User dashboard with warranty status overview
   - Visual indicators for active, expiring, and expired warranties
   - Statistics cards showing totals and counts
   - Responsive UI using Bootstrap

4. **Notification & Reminder System** ⚠️ (In Development)
   - Dashboard alerts for soon-to-expire warranties (partially implemented)
   - Email reminders for warranty expiration (planned)
   - Background service for automated notifications (not implemented yet)
   - Notification history and status tracking (basic structure in place)

5. **Service Record Management** ✅
   - Log service visits with vendor details
   - Track service costs and maintenance notes
   - Service history for each appliance

### Admin Features

1. **System Monitoring** ✅
   - View system statistics (total users, appliances, expiring warranties)
   - Monitor all appliances across users
   - Filter and search system-wide data

2. **User Management** ✅
   - View all registered users
   - Manage user accounts and roles
   - System administration capabilities

### Additional Features

- **Image Upload**: Upload and store purchase receipt images ✅
- **Email Integration**: Automated email notifications (planned for future release)
- **Responsive Design**: Mobile-friendly UI with Bootstrap ✅
- **Data Validation**: Client-side and server-side validation ✅

## Technical Implementation

### Technology Stack
- **Framework**: ASP.NET Core 3.1 MVC
- **Runtime**: .NET Core 3.1
- **Database**: SQL Server with Entity Framework Core 3.1
- **Authentication**: ASP.NET Core Identity 3.1
- **UI Framework**: Bootstrap 4 with Razor Views
- **Email Service**: MailKit (for future implementation)
- **Background Services**: IHostedService (planned for automated reminders)

### Architecture Pattern
- **MVC Pattern**: Proper separation of concerns with Controllers, Models, and Views
- **Repository Pattern**: Data access layer abstraction
- **Service Layer**: Business logic separation
- **Dependency Injection**: Built-in .NET Core DI container

### Database Schema

#### Core Entities:
- **AspNetUsers**: Identity framework users
- **Appliances**: Product warranty information
- **ServiceRecords**: Maintenance and service history
- **Notifications**: User notifications and alerts (basic structure)

#### Entity Relationships:
```
User (1) ───────── (M) ProductWarranty (1) ───────── (M) ServiceRecord
   │                   │
   │                   └── ProductName, PurchaseDate, WarrantyEndDate
   │
   └── Role (User/Admin)

ProductWarranty (1) ──── (M) Notification (basic structure)
```

## Setup Instructions

### Prerequisites
- .NET Core 3.1 SDK
- SQL Server or SQL Server LocalDB
- Visual Studio 2019 or later / VS Code

### Installation Steps

1. **Clone the Repository**
   ```shell
   git clone https://github.com/DeepVaishnav17/WarrantyTrackerApp.git
   cd WarrantyTrackerApp
   ```

2. **Restore Dependencies**
   ```shell
   dotnet restore
   ```

3. **Database Setup**
   
   **Drop & Recreate Database:**
   ```shell
   Drop-Database
   Update-Database
   ```
   
   Or using .NET Core CLI:
   ```shell
   dotnet ef database drop
   dotnet ef database update
   ```

4. **Configure Connection String**
   - Update `appsettings.json` with your SQL Server connection string
   - Default uses LocalDB for development

5. **Run the Application**
   ```shell
   dotnet run
   ```

6. **Access the Application**
   - Navigate to `https://localhost:5001` or `http://localhost:5000`
   - Register a new account or use admin credentials

### Admin Access

**Default Admin Credentials:**
- **Email**: `admin@warranty.com`
- **Password**: `Admin@123`

*Note: Check `Data/RoleSeeder.cs` for admin role configuration*

### Configuration
- **Email Settings**: SMTP settings in `appsettings.json` (for future email notifications)
- **File Upload**: Configure file upload paths for receipt images
- **Background Services**: Notification service planned for future implementation

## Development Status

### Completed Features ✅
- User Authentication and Authorization
- Appliance CRUD Operations
- Dashboard with warranty status display
- Service Record Management
- Admin panel with system statistics
- Responsive UI design
- Image upload functionality

### In Progress ⚠️
- **Notification System**: Basic structure implemented, full functionality in development
  - Dashboard notifications partially working
  - Email notifications planned for next release
  - Background service for automated reminders not yet implemented

### Planned Features 📋
- Complete email notification system
- Automated background reminder service
- Advanced filtering and search
- CSV export/import functionality
- Calendar integration

## Team Members and Individual Contributions

### Team Members
- **[Member 1 - Ronak Maniya]**: Authentication system, user management, database design
- **[Member 2 - Vivan Desai]**: Appliance CRUD operations, dashboard UI, responsive design
- **[Member 3 - Deep Vaishnav]**: Admin features, service records, notification system foundation

### Individual Contributions
- **Authentication & Security**: Implementation of ASP.NET Identity, role-based authorization
- **Frontend Development**: Responsive UI design, Bootstrap integration, user experience
- **Backend Services**: Data management, admin functionality, notification system structure
- **Database Design**: Entity relationships, migrations, data seeding
- **Testing & Integration**: Unit testing, system integration, deployment preparation

## Future Scope & Enhancements

### Next Release (v2.0)
- **Complete Notification System**: Full email integration and automated reminders
- **Background Services**: Scheduled tasks for warranty expiration checks
- **Advanced Search**: Enhanced filtering and sorting capabilities
- **Performance Optimization**: Caching and query optimization

### Long-term Vision
- **Mobile Application**: Native mobile app for iOS/Android
- **Calendar Integration**: Google Calendar/Outlook integration for reminders
- **Advanced Analytics**: Warranty trends and insights
- **Multi-language Support**: Internationalization features
- **API Integration**: Third-party integrations with manufacturers

### Scalability Improvements
- **Microservices Architecture**: Breaking down into smaller services
- **Cloud Deployment**: Azure/AWS hosting with auto-scaling
- **Real-time Notifications**: SignalR for live updates

## Project Structure

```
WarrantyTrackerApp/
├── Controllers/           # MVC Controllers
├── Models/               # Data models and ViewModels
├── Views/                # Razor views and layouts
├── Data/                 # Database context and seeders
├── Services/             # Business logic services
├── Areas/                # Admin area controllers and views
├── wwwroot/              # Static files (CSS, JS, images)
├── Migrations/           # EF Core migrations
├── Startup.cs            # Application configuration
└── Program.cs            # Application entry point
```

## Known Limitations

- **Email Notifications**: Not fully implemented yet
- **Background Services**: Automated reminder service pending
- **Notification History**: Basic structure exists but needs enhancement
- **File Upload Validation**: Limited file type and size validation

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project is developed as part of the Web Application Development course requirements.

---

**Course**: Web Application Development (MVC using .NET Core)  
**Professor**: Prof. Apurva Mehta  
**Academic Year**: 2024-27, Semester 5  
**Framework**: .NET Core 3.1
