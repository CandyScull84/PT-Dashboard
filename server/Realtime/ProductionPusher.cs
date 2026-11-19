using FullstackApp1.server.DTO;
using FullstackApp1.server.Services;
using FullstackApp1.server.Realtime;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Security.Cryptography;

namespace FullstackApp1.server.Realtime;

public sealed class ProductionPusher : BackgroundService
{
    private readonly IHubContext<DashboardHub> _hub;
    private readonly ProductionBuilder _builder;
    // private readonly SnapshotService _snapshotService;

    public ProductionPusher(IHubContext<DashboardHub> hub, ProductionBuilder builder, SnapshotService snapshotService)
    {
        _hub = hub;
        _builder = builder;
        // _snapshotService = snapshotService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // string? lastHash = null;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                //bygg production
                var series = await _builder.BuildTodayAsync();
                 await _hub.Clients.All.SendAsync("productionUpdate", series, cancellationToken: stoppingToken);
                //Skicka bara om det ändrats
                // var json = JsonSerializer.Serialize(production);
                // var hash = json.GetHashCode().ToString();
                // var hash = Fingerprint(series);
                // if (hash != lastHash)
                // {
                //     lastHash = hash;
                //     await _hub.Clients.All.SendAsync("productionUpdate", series, cancellationToken: stoppingToken);
                //     _snapshotService.InvalidateToday();
                // }
            }
            catch (Exception error)
            {
               Console.WriteLine($"ProductionPusher error: {error.Message}");
            }
            //intervall 
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private static string Fingerprint(ProductionSeriesDto s) {
        var sb = new StringBuilder();
        sb.Append(string.Join(',', s.Labels));
        foreach (var line in s.Lines.OrderBy(l => l.DeptId))
            sb.Append('|').Append(line.DeptId).Append(':').Append(string.Join(',', line.Values));

        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexString(bytes);
    }
}