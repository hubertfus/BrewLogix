using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models;

public class Ingredient : BaseEntity
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Type is required")]
    [StringLength(50, ErrorMessage = "Type can't be longer than 50 characters")]
    public string Type { get; set; } 
    
    [Required(ErrorMessage = "Unit is required")]
    [StringLength(20, ErrorMessage = "Unit can't be longer than 20 characters")]
    public string Unit { get; set; }


    public ICollection<RecipeIngredient> UsedInRecipes { get; set; }
    public ICollection<StockEntry> StockEntries { get; set; }
}
