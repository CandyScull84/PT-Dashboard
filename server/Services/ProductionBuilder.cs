// Services/ProductionBuilder.cs
using FullstackApp1.server.Data;
using FullstackApp1.server.DTO;
using System.Globalization;

namespace FullstackApp1.server.Services;

public sealed class ProductionBuilder
{
    private readonly OrderRepository _repo;
    public ProductionBuilder(OrderRepository repo) => _repo = repo;

    public async Task<ProductionSeriesDto> BuildTodayAsync()
    {
        var orders = await _repo.GetOrdersAsync();
        var currentTotals = await _repo.GetGroupTotalsAsync();

        var now = DateTime.Now;
        var today = new DateTime(now.Year, now.Month, now.Day);
        var start = today.AddHours(6);
        var end = today.AddHours(17);
        var dateKey = today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        var startTotals = await _repo.GetGroupTotalsAtCutoffAsync(start)
            ?? Array.Empty<GroupSummaryDto>();
        // Hämta fryst startTotals (samma som snapshot använder)
        //IReadOnlyList<GroupSummaryDto> startTotals;
        //if (!StartTotalsCache.TryGet(dateKey, out startTotals))
        //{
        //    if (now < start)
        //    {
        //        startTotals = Array.Empty<GroupSummaryDto>();
        //    }
        //    else
        //    {
        //        // Säkerställ att vi hämtar totals per 06:00 (inte “nu”)
        //        startTotals = await _repo.GetGroupTotalsAtCutoffAsync(start);
        //        StartTotalsCache.Set(dateKey, startTotals);
        //    }
        //}

        // Runda ner till närmaste 5:e minut (linjen slutar vid “nu”)
        static DateTime RoundDownTo5(DateTime t)
        {
            var m5 = (t.Minute / 5) * 5;
            return new DateTime(t.Year, t.Month, t.Day, t.Hour, m5, 0);
        }
        var nowSlot = RoundDownTo5(now);

        // Etiketter (5-min), frontend visar t.ex. bara hel timme
        var labels = new List<string>();
        for (var t = start; t <= end; t = t.AddMinutes(5))
            labels.Add(t.ToString("HH:mm", CultureInfo.InvariantCulture));

        bool IsFinishedBy(DateTime? finished, DateTime cutoff)
        => finished is not null && finished.Value.Date == today.Date && finished.Value <= cutoff;

        var deptIds = startTotals.Select(s => s.WorkgroupId).Distinct().OrderBy(x => x).ToList();
        var lines = new List<ProductionLineDto>();

        foreach (var deptId in deptIds)
        {
            var startTotal = startTotals.FirstOrDefault(s => s.WorkgroupId == deptId)?.TotalOrders ?? 0;
            var groupOrders = orders.Where(o => o.WorkgroupId == deptId).ToList();

            var values = new List<int?>();

            for (var cutoff = start; cutoff <= end; cutoff = cutoff.AddMinutes(5))
            {
                if (cutoff > nowSlot) break; // framtid -> stoppa linjen vid “nu”

                var completed = groupOrders.Count(o => IsFinishedBy(o.TimeFinished, cutoff));
                var remaining = Math.Max(0, startTotal - completed);
                values.Add(remaining);

                if (remaining == 0) break; // sluta rita när noll uppnåtts
            }

            lines.Add(new ProductionLineDto { DeptId = deptId, Values = values });
        }

        return new ProductionSeriesDto { Labels = labels, Lines = lines };
    }
}

