namespace BrewLogix.Models;

public class RecipeIngredient : BaseEntity
{
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }

    public decimal Quantity { get; set; }
        
    public int SelectedStockEntryId { get; set; } 

}
