using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Evently.Modules.Ticketing.Infrastructure.Database;


//dotnet ef migrations add Create_Database_Tic --project "C:\proj_cs\eventlyFinal\src\Modules\Ticketing\Evently.Modules.Ticketing.Infrastructure\Evently.Modules.Ticketing.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --output-dir "Database/Migrations" --context TicketingDbContext --verbose

//dotnet ef database update --project "C:\proj_cs\eventlyFinal\src\Modules\Ticketing\Evently.Modules.Ticketing.Infrastructure\Evently.Modules.Ticketing.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --context TicketingDbContext


//remove : dotnet ef migrations remove --project "C:\proj_cs\eventlyFinal\src\Modules\Ticketing\Evently.Modules.Ticketing.Infrastructure\Evently.Modules.Ticketing.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --context TicketingDbContext
public class DesignTimeTicketingDbContextFactory : IDesignTimeDbContextFactory<TicketingDbContext>
{
    public TicketingDbContext CreateDbContext(string[] args)
    {
        // Build configuration (looks for appsettings.json in the startup project or current dir)
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())           // or Path.Combine(..., "src", "API", "Evently.Api")
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("Database");

        var optionsBuilder = new DbContextOptionsBuilder<TicketingDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(TicketingDbContext).Assembly.FullName);
            // Fix for the "encrypt" error
            sqlOptions.EnableRetryOnFailure();
        });

        return new TicketingDbContext(optionsBuilder.Options);
    }
}
