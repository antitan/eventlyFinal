using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace Evently.Modules.Users.Infrastructure.Database;

//1- set as startup project

//dotnet ef migrations add Create_Database --project "C:\proj_cs\eventlyFinal\src\Modules\Users\Evently.Modules.Users.Infrastructure\Evently.Modules.Users.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --output-dir "Database/Migrations" --context UsersDbContext --verbose

//dotnet ef database update --project "C:\proj_cs\eventlyFinal\src\Modules\Users\Evently.Modules.Users.Infrastructure\Evently.Modules.Users.Infrastructure.csproj" --startup-project "C:\proj_cs\eventlyFinal\src\API\Evently.Api\Evently.Api.csproj" --context UsersDbContext


public class DesignTimeUsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        // Build configuration (looks for appsettings.json in the startup project or current dir)
        IConfigurationRoot? configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())           // or Path.Combine(..., "src", "API", "Evently.Api")
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("Database");

        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName);
            // Fix for the "encrypt" error
            sqlOptions.EnableRetryOnFailure();
        });

        return new UsersDbContext(optionsBuilder.Options);
    }
}
