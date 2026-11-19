using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using FullstackApp1.server.Data;
using FullstackApp1.server.Services;

namespace FullstackApp1.server.HealthChecks;

public sealed class ServerHealthBackgroundService : BackgroundService
{
private readonly ILogger<ServerHealthBackgroundService> _log;
private readonly IHttpClientFactory _httpFactory;
private readonly DbConnectionFactory _dbFactory;
private readonly SnapshotService _snapshot;

public ServerHealthBackgroundService(
ILogger<ServerHealthBackgroundService> log,
IHttpClientFactory httpFactory,
DbConnectionFactory dbFactory,
SnapshotService snapshot)
{
_log = log;
_httpFactory = httpFactory;
_dbFactory = dbFactory;
_snapshot = snapshot;
}

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
// Vänta lite så webben hinner starta
await Task.Delay(1500, stoppingToken);

var client = _httpFactory.CreateClient("self");

// 1) Testa HTTP GET "/" (ska ge HTML/200). Kräver inte auth.
try
{
var res = await client.GetAsync("/", stoppingToken);
_log.LogInformation("GET / → {Code}", (int)res.StatusCode);
}
catch (Exception ex)
{
_log.LogError(ex, "GET / misslyckades");
}

// 2) Testa en enkel health-endpoint (AllowAnonymous)
try
{
var res = await client.GetAsync("/health", stoppingToken);
var ok = res.IsSuccessStatusCode;
_log.LogInformation("GET /health → {Code} (ok={Ok})", (int)res.StatusCode, ok);
}
catch (Exception ex)
{
_log.LogError(ex, "GET /health misslyckades");
}

// 3) DB-anslutning direkt (förbi auth). Kräver endast att connection string funkar.
try
{
await using var c = _dbFactory.Create();
await c.OpenAsync(stoppingToken);
await using var cmd = c.CreateCommand();
cmd.CommandText = "SELECT TOP (1) 1";
var v = (int)(await cmd.ExecuteScalarAsync(stoppingToken))!;
_log.LogInformation("DB ping OK (value={Value})", v);
}
catch (Exception ex)
{
_log.LogError(ex, "DB ping misslyckades");
}

// 4) Testa att vyn finns (direkt mot DB)
try
{
await using var c = _dbFactory.Create();
await c.OpenAsync(stoppingToken);
await using var cmd = c.CreateCommand();
cmd.CommandText = "SELECT TOP(1) 1 FROM dbo.vw_ProductionOrders";
var v = await cmd.ExecuteScalarAsync(stoppingToken);
_log.LogInformation("View ping OK (sample={Sample})", v);
}
catch (Exception ex)
{
_log.LogError(ex, "View ping misslyckades");
}

// 5) Bygg Snapshot i minnet (förbi auth) – verifierar din domänlogik
try
{
var snap = await _snapshot.BuildSnapshotAsync();
var json = JsonSerializer.Serialize(new {
snap.TotalAllOrders,
Groups = snap.TotalsStart.Count,
Lines = snap.Production?.Lines?.Count ?? 0
});
_log.LogInformation("Snapshot byggd OK → {Info}", json);
}
catch (Exception ex)
{
_log.LogError(ex, "Snapshot build misslyckades");
}

// 6) (Frivilligt) testa SignalR negotiate via HTTP – OBS: kan kräva auth → hoppa om det stör
try
{
var req = new HttpRequestMessage(HttpMethod.Post, "/hub/negotiate");
req.Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");
var res = await client.SendAsync(req, stoppingToken);
_log.LogInformation("POST /hub/negotiate → {Code}", (int)res.StatusCode);
}
catch (Exception ex)
{
_log.LogWarning(ex, "SignalR negotiate test hoppades/failed (kan kräva auth)");
}

_log.LogInformation("Health checks klara ✅");
}
}
