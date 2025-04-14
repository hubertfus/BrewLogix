using System.ComponentModel.DataAnnotations;
using BrewLogix.Validation;

namespace BrewLogix.Models;

public class Recipe : BaseEntity
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Style is required")]
    [StringLength(50, ErrorMessage = "Style can't be longer than 50 characters")]
    public string Style { get; set; }
    
    [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
    public string? Description { get; set; }

    [AtLeastOneIngredient(ErrorMessage = "At least one ingredient is required")]
    [UniqueIngredients(ErrorMessage = "Each ingredient can only be used once in a recipe")]
    public ICollection<RecipeIngredient> Ingredients { get; set; }
    public ICollection<Batch> Batches { get; set; }
}
