namespace BrewLogix.Models;

public class Client : BaseEntity
{
    public string Name { get; set; }
    public string ContactEmail { get; set; }
    public string Address { get; set; }

    public ICollection<Order> Orders { get; set; }
}
