using Evently.Common.Infrastructure.Inbox;
using Evently.Common.Infrastructure.Outbox;
using Evently.Modules.Attendance.Application.Abstractions.Data;
using Evently.Modules.Attendance.Domain.Attendees;
using Evently.Modules.Attendance.Domain.Events;
using Evently.Modules.Attendance.Domain.Tickets;
using Evently.Modules.Attendance.Infrastructure.Attendees;
using Evently.Modules.Attendance.Infrastructure.Events;
using Evently.Modules.Attendance.Infrastructure.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Evently.Modules.Attendance.Infrastructure.Database;

public sealed class AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<Attendee> Attendees { get; set; }

    internal DbSet<Event> Events { get; set; }

    internal DbSet<Ticket> Tickets { get; set; }

    internal DbSet<EventStatistics> EventStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Attendance);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new AttendeeConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new TicketConfiguration());
        modelBuilder.ApplyConfiguration(new EventStatisticsConfiguration());
    }

    public override int SaveChanges()
    {
        try
        {
            return base.SaveChanges();
        }
        catch (Exception exception)
        {
            this.GetService<ILogger<AttendanceDbContext>>().LogError(
                exception,
                "{ClassName}:{MethodName} - Erreur EF Core pendant SaveChanges.",
                nameof(AttendanceDbContext),
                nameof(SaveChanges));
            throw;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (Exception exception)
        {
            this.GetService<ILogger<AttendanceDbContext>>().LogError(
                exception,
                "{ClassName}:{MethodName} - Erreur EF Core pendant SaveChangesAsync.",
                nameof(AttendanceDbContext),
                nameof(SaveChangesAsync));
            throw;
        }
    }
}
