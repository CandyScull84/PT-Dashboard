namespace FullstackApp1.server.DTO;

public class OrderItemDto
{
    public int WorkgroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime? TimeFinished { get; set; }

    public double? TotalTime { get; set; }
}
