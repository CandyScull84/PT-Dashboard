using Microsoft.AspNetCore.Server.IISIntegration;
using FullstackApp1.server.Data;
using FullstackApp1.server.Realtime;
using FullstackApp1.server.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ======= Logger =======
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog(options =>
    {
        options.SourceName = "DashboardApp";
    });
}


// ======= Auth (Windows) =======
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(options =>
{
    var requiredGroup = builder.Configuration["Authorization:RequiredGroup"];
    options.AddPolicy("RequireADGroup", policy =>
    policy.RequireAssertion(ctx =>
    {
        if (string.IsNullOrWhiteSpace(requiredGroup))
            return ctx.User.Identity?.IsAuthenticated == true;
        return ctx.User.IsInRole(requiredGroup);
    }));
});

// ======= CORS =======
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowSameOrigin", p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
});

// ======= Services =======
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddSingleton<OrderRepository>();
builder.Services.AddSingleton<SnapshotService>();
builder.Services.AddSingleton<ProductionBuilder>();
builder.Services.AddHostedService<ProductionPusher>();
builder.Services.AddSignalR().AddJsonProtocol();
//builder.Services.AddMemoryCache();

var app = builder.Build();

// ======= Base path (/dashboard) =======
app.UsePathBase("/dashboard");

// ======= Error handling =======
app.UseExceptionHandler("/error");
app.MapGet("/error", () => Results.Problem(statusCode: 500, title: "Ett fel inträffade"));

// ======= Static files =======
app.UseDefaultFiles();
app.UseStaticFiles();

// ======= Middleware order =======
app.UseCors("AllowSameOrigin");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseDeveloperExceptionPage();


// ======= API =======

app.MapGet("/_routes", (IEnumerable<EndpointDataSource> sources) =>
sources.SelectMany(s => s.Endpoints)
.OfType<RouteEndpoint>()
.Select(e => e.RoutePattern.RawText));

Require(app.MapGet("/api/me", (ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        authenticated = user.Identity?.IsAuthenticated ?? false,
        name = user.Identity?.Name
    });
}));

Require(app.MapGet("/api/orders", async (OrderRepository repo) =>
{
    var items = await repo.GetOrdersAsync();
    return Results.Ok(items);
}));

Require(app.MapGet("/api/totals", async (OrderRepository repo) =>
{
    var totals = await repo.GetGroupTotalsAsync();
    return Results.Ok(totals);
}));

Require(app.MapGet("/api/totalall", async (OrderRepository repo) =>
{
    var totalAll = await repo.GetOverallTotalAsync();
    return Results.Ok(new { totalAll });
}));

Require(app.MapGet("/api/snapshot", async (SnapshotService service, ILoggerFactory lf) =>
{
    try
    {
        var snapshot = await service.BuildSnapshotAsync();
        return Results.Ok(snapshot);
    }
    catch (Exception ex)
    {
        var log = lf.CreateLogger("Snapshot");
        log.LogError(ex, "BuildSnapshotAsync failed");
        return Results.Problem(
            title: "Snapshot failed",
            detail: ex.Message,
            statusCode: 500);
    }
}));

// ======= Healthcheck =======
Require(app.MapGet("/api/health", async (OrderRepository repo, SnapshotService snapshot) =>
{
    bool dbOk = false;
    bool snapshotOk = false;

    try
    {
        await repo.GetOverallTotalAsync();
        dbOk = true;
    }
    catch { }

    try
    {
        await snapshot.BuildSnapshotAsync();
        snapshotOk = true;
    }
    catch { }

    return Results.Ok(new
    {
        dbOk,
        snapshotOk,
        serverTime = DateTime.Now
    });
}));

// ======= SignalR hub =======
Require(app.MapHub<DashboardHub>("/hub"));

// ======= SPA fallback =======
app.MapFallbackToFile("index.html");

app.Run();


// ======= Helper for RequireAuthorization =======
static T Require<T>(T endpoint) where T : IEndpointConventionBuilder
{
    endpoint.RequireAuthorization("RequireADGroup");
    return endpoint;
}

