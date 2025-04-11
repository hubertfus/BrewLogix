namespace BrewLogix.Models;

public class Batch : BaseEntity
{
    public string Code { get; set; }

    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string Status { get; set; } 

    public ICollection<BatchLog> Logs { get; set; }
    public ICollection<Keg> Kegs { get; set; }
}
