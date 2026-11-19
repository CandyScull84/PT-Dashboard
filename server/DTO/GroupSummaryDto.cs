using Microsoft.Identity.Client;

namespace FullstackApp1.server.DTO;

public sealed record GroupSummaryDto {
     public int WorkgroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public int TotalOrders { get; set; }//dagens totalantal Ordrar per grupp
}

