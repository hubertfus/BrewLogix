namespace BrewLogix.Models;

public class Order : BaseEntity
{
    public int ClientId { get; set; }
    public Client Client { get; set; }

    public DateTime OrderedAt { get; set; }
    public string Status { get; set; } 

    public ICollection<Keg> Kegs { get; set; }
}
