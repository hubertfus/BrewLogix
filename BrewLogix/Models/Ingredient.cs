namespace BrewLogix.Models;

public class Ingredient : BaseEntity
{
    public string Name { get; set; }
    public string Type { get; set; } 
    public string Unit { get; set; }

    public ICollection<RecipeIngredient> UsedInRecipes { get; set; }
    public ICollection<StockEntry> StockEntries { get; set; }
}
