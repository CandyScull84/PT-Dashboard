// Services/SnapshotService.cs
using FullstackApp1.server.DTO;
using FullstackApp1.server.Data;
using System.Globalization;

namespace FullstackApp1.server.Services;

public class SnapshotService
{
    private readonly OrderRepository _repo;
    private readonly ProductionBuilder _production;

    public SnapshotService(OrderRepository repo, ProductionBuilder production)
    {
        _repo = repo;
        _production = production;
    }

    public async Task<SnapshotDto> BuildSnapshotAsync()
    {
        // Basdata (nu-läget)
        var orders = await _repo.GetOrdersAsync();
        var currentTotals = await _repo.GetGroupTotalsAsync();
        var totalAll = await _repo.GetOverallTotalAsync();

        var now = DateTime.Now;
        var today = new DateTime(now.Year, now.Month, now.Day);
        var start = today.AddHours(6); // 06:00
        var dateKey = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        //✅ Hämtar startvärden från cutoff kl 06:00
        var startTotals = await _repo.GetGroupTotalsAtCutoffAsync(start)
            ?? Array.Empty<GroupSummaryDto>();

        // FRYST startvärde = totals vid exakt 06:00
        //IReadOnlyList<GroupSummaryDto> startTotals;
        //if (!StartTotalsCache.TryGet(dateKey, out startTotals))
        //{
        //    if (now < start)
        //    {
        //        // Före 06:00: vi visar tomt startläge och väntar med att freeze:a
        //        startTotals = Array.Empty<GroupSummaryDto>();
        //    }
        //    else
        //    {
        //        // Första gången efter 06:00: hämta totals som gällde kl 06:00 och frys in
        //        startTotals = await _repo.GetGroupTotalsAtCutoffAsync(start);
        //        StartTotalsCache.Set(dateKey, startTotals);
        //    }
        //}

        // Linjeserie (bygger alltid från fryst startTotals)
        var production = await _production.BuildTodayAsync();

        // End-of-day % (räknat relativt fryst startTotals)
        bool FinishedToday(DateTime? f) => f is not null && f.Value.Date == today.Date;

        var endOfDay = startTotals
        .OrderBy(s => s.WorkgroupId)
        .Select(s =>
        {
            var deptId = s.WorkgroupId;
            var startCnt = s.TotalOrders;
            var completed = orders.Count(o => o.WorkgroupId == deptId && FinishedToday(o.TimeFinished));
            var pctRaw = startCnt == 0 ? 0 : 100.0 * completed / startCnt;
            var pct = (int)Math.Min(100, Math.Round(pctRaw, MidpointRounding.AwayFromZero));
            return new EndOfDayStatDto { DeptId = deptId, CompletedPercent = pct };
            //var pct = startCnt == 0 ? 0 :
            //(int)Math.Min(100, Math.Round(100.0 * completed / startCnt));
        })
        .ToList();

        return new SnapshotDto
        {
            DateLocal = dateKey,
            TotalAllOrders = totalAll,
            Totals = currentTotals.ToList(), // valfritt “nu-läge”
            TotalsStart = startTotals.ToList(), // <- DONUT + TABELL (FRYST)
            Orders = orders.ToList(),
            Signature = Guid.NewGuid().ToString("N"),
            Production = production, // linjen använder samma frysta start
            EndOfDay = endOfDay
        };
    }
}
