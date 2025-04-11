namespace BrewLogix.Models;

public class BatchLog : BaseEntity
{
    public int BatchId { get; set; }
    public Batch Batch { get; set; }

    public DateTime Timestamp { get; set; }
    public string Note { get; set; }
}