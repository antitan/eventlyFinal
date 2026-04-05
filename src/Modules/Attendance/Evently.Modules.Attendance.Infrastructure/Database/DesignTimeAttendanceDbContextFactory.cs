using Evently.Modules.Attendance.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Evently.Modules.Users.Infrastructure.Database;

//1- set as startup project

//dotnet ef migrations add Create_Database_Att --project "C:\proj_cs\eventlyFinal\src\Modules\Attendance\Evently.Modules.Attendance.Infrastructure\Evently.Modules.Attendance.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --output-dir "Database/Migrations" --context AttendanceDbContext --verbose

//dotnet ef database update --project "C:\proj_cs\eventlyFinal\src\Modules\Attendance\Evently.Modules.Attendance.Infrastructure\Evently.Modules.Attendance.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --context AttendanceDbContext


public class DesignTimeAttendanceDbContextFactory : IDesignTimeDbContextFactory<AttendanceDbContext>
{
    public AttendanceDbContext CreateDbContext(string[] args)
    {
        // Build configuration (looks for appsettings.json in the startup project or current dir)
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())           // or Path.Combine(..., "src", "API", "Evently.Api")
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("Database");

        var optionsBuilder = new DbContextOptionsBuilder<AttendanceDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(AttendanceDbContext).Assembly.FullName);
            // Fix for the "encrypt" error
            sqlOptions.EnableRetryOnFailure();
        });

        return new AttendanceDbContext(optionsBuilder.Options);
    }
}
