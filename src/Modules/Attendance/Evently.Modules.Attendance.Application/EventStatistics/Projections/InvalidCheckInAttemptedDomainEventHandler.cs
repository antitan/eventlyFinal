using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Modules.Attendance.Domain.Attendees;

namespace Evently.Modules.Attendance.Application.EventStatistics.Projections;

internal sealed class InvalidCheckInAttemptedDomainEventHandler(IDbConnectionFactory dbConnectionFactory)
    : DomainEventHandler<InvalidCheckInAttemptedDomainEvent>
{
    public override async Task Handle(
        InvalidCheckInAttemptedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        using System.Data.IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            UPDATE attendance.event_statistics
            SET invalid_check_in_tickets = JSON_MODIFY(
                CASE
                    WHEN ISJSON(invalid_check_in_tickets) = 1 THEN invalid_check_in_tickets
                    ELSE '[]'
                END,
                'append $',
                @TicketCode)
            WHERE event_id = @EventId
            """;

        await connection.ExecuteAsync(sql, domainEvent);
    }
}
