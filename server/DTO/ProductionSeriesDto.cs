namespace FullstackApp1.server.DTO;

public class ProductionSeriesDto
{
    public List<String> Labels { get; set; } = new(); //Arbetstimmar
    public List<ProductionLineDto> Lines { get; set; } = new(); //Serie per avdelning
}

public class ProductionLineDto
{
    public int DeptId { get; set; } 
    public List<int?> Values { get; set; } = new(); //Kvarvarande per timme
}
