using Evently.Modules.Ticketing.Domain.Events;
using Evently.Modules.Ticketing.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Evently.Modules.Ticketing.Infrastructure.Orders;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.HasOne<Order>().WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TicketType>().WithMany().HasForeignKey(oi => oi.TicketTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
