namespace FullstackApp1.server.DTO;

public class SnapshotDto
{
    public string DateLocal { get; set; } = string.Empty;
    public int TotalAllOrders { get; set; }
    public List<GroupSummaryDto> Totals { get; set; } = new();

    public List<GroupSummaryDto> TotalsStart { get; set; } = new();
    public List<OrderItemDto> Orders { get; set; } = new();
    public string Signature { get; set; } = string.Empty;

    public ProductionSeriesDto Production { get; set; } = new();

    public List<EndOfDayStatDto> EndOfDay { get; set; } = new();

}