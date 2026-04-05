using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Evently.Modules.Events.Infrastructure.Database;
//1- set as startup project

//dotnet ef migrations add Create_Database_Evt --project "C:\proj_cs\eventlyFinal\src\Modules\Events\Evently.Modules.Events.Infrastructure\Evently.Modules.Events.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --output-dir "Database/Migrations" --context EventsDbContext --verbose

//dotnet ef database update --project "C:\proj_cs\eventlyFinal\src\Modules\Attendance\Evently.Modules.Attendance.Infrastructure\Evently.Modules.Attendance.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --context EventsDbContext


public class DesignTimeEventsDbContextFactory : IDesignTimeDbContextFactory<EventsDbContext>
{
    public EventsDbContext CreateDbContext(string[] args)
    {
        // Build configuration (looks for appsettings.json in the startup project or current dir)
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())           // or Path.Combine(..., "src", "API", "Evently.Api")
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("Database");

        var optionsBuilder = new DbContextOptionsBuilder<EventsDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(EventsDbContext).Assembly.FullName);
            // Fix for the "encrypt" error
            sqlOptions.EnableRetryOnFailure();
        });

        return new EventsDbContext(optionsBuilder.Options);
    }
}
