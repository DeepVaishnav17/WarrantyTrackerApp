# 1. Drop & Recreate Database:

```shell
Drop-Database
Update-Database
```

# 2. About Admin Role

Check Data/RoleSeeder.cs

```shell
Name = "Admin"
Email = "admin@warranty.com"
Password = "Admin@123"
```

Make sure NotificationService.CheckWarrantyStatusesAsync is called periodically (like on login or a scheduled job) so LastWarrantyStatus is up-to-date for display and SignalR notifications.
