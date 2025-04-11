namespace BrewLogix.Models;

public class StockEntry : BaseEntity
{
    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; }

    public decimal Quantity { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
