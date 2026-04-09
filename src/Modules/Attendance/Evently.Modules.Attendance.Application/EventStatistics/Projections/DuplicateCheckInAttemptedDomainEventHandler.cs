using System.Data;
using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Modules.Attendance.Domain.Attendees;

namespace Evently.Modules.Attendance.Application.EventStatistics.Projections;

internal sealed class DuplicateCheckInAttemptedDomainEventHandler(IDbConnectionFactory dbConnectionFactory)
    : DomainEventHandler<DuplicateCheckInAttemptedDomainEvent>
{
    public override async Task Handle(
        DuplicateCheckInAttemptedDomainEvent domainEvent,
        CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            UPDATE attendance.event_statistics
            SET duplicate_check_in_tickets = JSON_MODIFY(
                CASE
                    WHEN ISJSON(duplicate_check_in_tickets) = 1 THEN duplicate_check_in_tickets
                    ELSE '[]'
                END,
                'append $',
                @TicketCode)
            WHERE event_id = @EventId
            """;

        await connection.ExecuteAsync(sql, domainEvent);
    }
}
