using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models
{
    public class RecipeIngredient : BaseEntity
    {
        [Required(ErrorMessage = "Recipe is required")]
        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }

        [Required(ErrorMessage = "Ingredient is required")]
        public int IngredientId { get; set; }

        public Ingredient Ingredient { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Stock entry must be selected")]
        public int SelectedStockEntryId { get; set; }
    }
}