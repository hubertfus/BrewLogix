using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models;

public class StockEntry : BaseEntity
{      
    [Required(ErrorMessage = "Ingredient is required")]
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }
    
    [Required(ErrorMessage = "Delivery date is required")]
    [DataType(DataType.Date)]
    public DateTime DeliveryDate { get; set; }
    
    [Required(ErrorMessage = "Expiry date is required")]
    [DataType(DataType.Date)]
    public DateTime ExpiryDate { get; set; }
}
