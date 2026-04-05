using Evently.Modules.Ticketing.Domain.Customers;
using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Ticketing.Domain.Orders;
using Evently.Modules.Ticketing.Domain.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Modules.Ticketing.Infrastructure.Tickets;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Code).HasMaxLength(30);
        builder.HasIndex(t => t.Code).IsUnique();

        // Seule FK en cascade (propriétaire principal)
        builder.HasOne<Customer>().WithMany().HasForeignKey(t => t.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Les autres en Restrict pour éviter les cycles
        builder.HasOne<Order>().WithMany().HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Event>().WithMany().HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TicketType>().WithMany().HasForeignKey(t => t.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
