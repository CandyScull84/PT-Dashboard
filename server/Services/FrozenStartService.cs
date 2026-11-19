//using FullstackApp1.server.Data;
//using FullstackApp1.server.DTO;
//using FullstackApp1.server.Services;
//using Microsoft.Extensions.Caching.Memory;
//using System.Globalization;
//using System.Collections.Concurrent;
//using System.Xml;

//namespace FullstackApp1.server.Services;

//public sealed class FrozenStartService
//{
//    private readonly IMemoryCache _cache;
//    private readonly OrderRepository _repo;
//    private readonly ProductionBuilder _production;

//    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(5); //Tid cacha snapshot
//    private static readonly ConcurrentDictionary<string, IReadOnlyList<GroupSummaryDto>> _startTotalsByDate = new();

//    public FrozenStartService(IMemoryCache cache, OrderRepository repo, ProductionBuilder production)
//    {
//        _cache = cache;
//        _repo = repo;
//        _production = production;
//    }

//    public async Task<SnapshotDto> GetAsync()
//    {
//        var todayKey = $"snapshot:{DateTime.Now:yyyy-MM-dd}";
//        var dateKey = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

//        var orders = await _repo.GetOrdersAsync();
//        var currentTotals = await _repo.GetGroupTotalsAsync();
//        var totalAll = await _repo.GetOverallTotalAsync();
//        var startTotals = _startTotalsByDate.GetOrAdd(dateKey, _ => currentTotals);

//        if (_cache.TryGetValue(todayKey, out SnapshotDto? cached) && cached is not null)
//            return cached;

//        var production = await _production.BuildTodayAsync();

//        //EndOfDay
//        var now = DateTime.Now;
//        var today = new DateTime(now.Year, now.Month, now.Day);
//        bool FinishedToday(DateTime? finished) => finished is not null && finished.Value.Date == today.Date;

//        var endOfDay = new List<EndOfDayStatDto>();
//        foreach (var summary in currentTotals.OrderBy(s => s.WorkgroupId))
//        {
//            var deptId = summary.WorkgroupId;
//            var total = summary.TotalOrders;
//            var completedCount = orders.Count(o => o.WorkgroupId == deptId && FinishedToday(o.TimeFinished));
//            var percent = total == 0 ? 0 : (int)Math.Round(100.0 * completedCount / total, MidpointRounding.AwayFromZero);
//            endOfDay.Add(new EndOfDayStatDto { DeptId = deptId, CompletedPercent = percent });
//        }
//        var snapshot = new SnapshotDto
//        {
//            DateLocal = today.ToString("yyyy-MM-dd"),
//            TotalAllOrders = totalAll,
//            Totals = currentTotals.ToList(),
//            TotalsStart = startTotals.ToList(),
//            Orders = orders.ToList(),
//            Signature = Guid.NewGuid().ToString("N"),
//            Production = production,
//            EndOfDay = endOfDay
//        };
//        //Cacha resultatet en kort stund
//        _cache.Set(todayKey, snapshot, CacheDuration);
//        return snapshot;

//    }
//    //Metod att nolla cache om man vill trigga direktpåslag från tex SignalR
//    public void InvalidateToday()
//    {
//        var todayKey = $"snapshot:{DateTime.Now:yyyy-MM-dd}";
//        _cache.Remove(todayKey);
//    }
//}

using Microsoft.Extensions.Caching.Memory;
using FullstackApp1.server.Data;
using FullstackApp1.server.DTO;

namespace FullstackApp1.server.Services;

public sealed class FrozenStartService
{
    private readonly IMemoryCache _cache;
    public FrozenStartService(IMemoryCache cache) => _cache = cache;

    public async Task<IReadOnlyList<GroupSummaryDto>> GetFrozenTotalsAsync(OrderRepository repo)
    {
        var now = DateTime.Now;
        var today = DateTime.Today;
        var start = today.AddHours(6);
        var end = today.AddHours(18);
        var key = $"frozen-start: {today:yyyy-MM-dd}";

        //ÄR totals redan fryst för dagen?
        if (_cache.TryGetValue(key, out IReadOnlyList<GroupSummaryDto>? cached) && cached is not null)
            return cached;

        //Innan arbetsdagen startar
        if (now < start)
            return Array.Empty<GroupSummaryDto>();

        //Läs in från DB och frys
        var totals = await repo.GetGroupTotalsAsync();
        _cache.Set(key, totals, new MemoryCacheEntryOptions {  AbsoluteExpiration = end });
        return totals;

    }
}

