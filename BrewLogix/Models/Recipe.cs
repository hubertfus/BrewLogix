namespace BrewLogix.Models;

public class Recipe : BaseEntity
{
    public string Name { get; set; }
    public string Style { get; set; }
    public string? Description { get; set; }

    public ICollection<RecipeIngredient> Ingredients { get; set; }
    public ICollection<Batch> Batches { get; set; }
}
