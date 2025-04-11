namespace BrewLogix.Models;

public class FermentationEntry : BaseEntity
{
    public int BatchId { get; set; }
    public Batch Batch { get; set; }

    public DateTime Date { get; set; }
    public float? TemperatureC { get; set; }
    public float? Gravity { get; set; }
}
