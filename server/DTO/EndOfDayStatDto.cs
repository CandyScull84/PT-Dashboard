namespace FullstackApp1.server.DTO;

public class EndOfDayStatDto
{
    public int DeptId { get; set; } 
    public int CompletedPercent { get; set; } // 0-100% avklarade ordrar idag
}