using Dapper;
using FullstackApp1.server.DTO;
using FullstackApp1.server.Data;

namespace FullstackApp1.server.Data;

/// <summary>
/// Repository för att hämta orderdata från databasen.
/// Läser från vyn dbo.vw_ProductionOrders och mappar till DTO-objekt.
/// </summary>
public sealed class OrderRepository
{
    private readonly DbConnectionFactory _factory;

    public OrderRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    /// <summary>
    /// Hämtar alla orderrader från vyn dbo.vw_ProductionOrders.
    /// Returnerar både klara och öppna ordrar.
    /// Kolumner: function_id (gruppnamn), operation (ordernummer), qty_units (antal),
    /// scala_workgroup (gruppnummer), time_finished (klar-tid).
    /// </summary>
    public async Task<IReadOnlyList<OrderItemDto>> GetOrdersAsync(CancellationToken ct = default)
    {
        await using var connection = _factory.Create();

        var sql = @"
        SELECT
            WorkgroupId = TRY_CAST(scala_workgroup AS int),
            GroupName = function_id,
            OrderNumber = operation,
            Quantity = qty_units,
            TimeFinished = time_finished,
            TotalTime = total_time
        FROM dbo.vw_ProductionOrders
        ORDER BY GroupName, OrderNumber;";

        var rows = await connection.QueryAsync<OrderItemDto>(sql);
        return rows.ToList();
    }

    /// <summary>
    /// Hämtar totalantalet öppna ordrar per arbetsgrupp.
    /// Endast ordrar där time_finished IS NULL räknas med.
    /// Används för att sätta startvärdet i produktionslinjen samt cirkeln.
    /// </summary>
    public async Task<IReadOnlyList<GroupSummaryDto>> GetGroupTotalsAsync(CancellationToken ct = default)
    {
        await using var connection = _factory.Create();
        var sql = @"
        SELECT
            WorkgroupId = TRY_CAST(scala_workgroup AS int),
            GroupName = MAX(function_id),
            TotalOrders = COUNT(*)
        FROM dbo.vw_ProductionOrders
        
        WHERE time_finished IS NULL 
        AND TRY_CAST(scala_Workgroup AS int) IS NOT NULL
        GROUP BY TRY_CAST (scala_workgroup AS int)
        ORDER BY WorkgroupId;";

        var rows = await connection.QueryAsync<GroupSummaryDto>(sql);
        return rows.ToList();
    }

        public async Task<IReadOnlyList<GroupSummaryDto>> GetGroupTotalsAtCutoffAsync(DateTime cutoffLocal, CancellationToken ct = default)
    {
        await using var connection = _factory.Create();
        var sql = @"
        SELECT
            WorkgroupId = TRY_CAST(scala_workgroup AS int),
            GroupName = MAX(function_id),
            -- Öppna kl 06:00 = alla ordrar som INTE var färdiga före cutoff
            TotalOrders = COUNT(*)
        FROM dbo.vw_ProductionOrders
        WHERE TRY_CAST(scala_workgroup AS int) IS NOT NULL 
        AND (time_finished IS NULL OR time_finished >= @cutoff)
        
        GROUP BY TRY_CAST (Scala_workgroup AS int)
        ORDER BY WorkgroupId;";

        var rows = await connection.QueryAsync<GroupSummaryDto>(sql, new { cutoff = cutoffLocal });
        return rows.ToList();
    }

    /// <summary>
    /// Hämtar totalt antal ordrar i databasen (både klara och öppna).
    /// Används för Slutresultatet.
    /// </summary>
    public async Task<int> GetOverallTotalAsync(CancellationToken ct = default)
    {
        await using var connection = _factory.Create();
        var sql = @"
        SELECT COUNT(*)
        FROM dbo.vw_ProductionOrders;";

        return await connection.ExecuteScalarAsync<int>(sql);
    }
}