namespace BrewLogix.Models;

public interface IDistributable
{
    bool IsDistributed { get; set; }
    DateTime FilledAt { get; set; }
}

public class Keg : BaseEntity, IDistributable
{
    public string Code { get; set; }
    public int BatchId { get; set; }
    public Batch Batch { get; set; }

    public string Size { get; set; } 
    public bool IsDistributed { get; set; }
    public DateTime FilledAt { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}
