using BrewLogix.Models;

namespace BrewLogix.Services;

public class StockEntriesService
{
    private readonly List<StockEntry> _stockEntries;
    private readonly IngredientService _ingredientService;

    public StockEntriesService(IngredientService ingredientService)
    {
        _ingredientService = ingredientService;
        var ingredients = _ingredientService.GetAllIngredients().ToList();
        _stockEntries = new List<StockEntry>
        {
            new StockEntry { Id = 1, Ingredient = ingredients[0], Quantity = 2, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(6) },
            new StockEntry { Id = 2, Ingredient = ingredients[1], Quantity = 200, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3) },
            new StockEntry { Id = 3, Ingredient = ingredients[0], Quantity = 2137, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3) },
            new StockEntry { Id = 4, Ingredient = ingredients[2], Quantity = 400, DeliveryDate = DateTime.Now.AddDays(-1), ExpiryDate = DateTime.Now.AddMonths(6) },
            new StockEntry { Id = 5, Ingredient = ingredients[3], Quantity = 100, DeliveryDate = DateTime.Now.AddDays(-2), ExpiryDate = DateTime.Now.AddMonths(5) }
        };
    }
    
    public IEnumerable<StockEntry> GetAllStockEntries() => _stockEntries;

    
    public IEnumerable<StockEntry> GetStockEntriesForIngredient(int ingredientId)
    {
        return _stockEntries.Where(se => se.Ingredient.Id == ingredientId);
    }
    
    public void AddStockEntry(StockEntry stockEntry)
    {
        stockEntry.Id = _stockEntries.Max(se => se.Id) + 1;
        _stockEntries.Add(stockEntry);
    }

    public void UpdateStockEntry(StockEntry stockEntry)
    {
        var index = _stockEntries.FindIndex(se => se.Id == stockEntry.Id);
        _stockEntries[index] = stockEntry;
    }
    
    public StockEntry GetStockEntry(int id) => _stockEntries.FirstOrDefault(se => se.Id == id);

    public void DeleteStockEntry(StockEntry stockEntry)
    {
        _stockEntries.Remove(stockEntry);
    }
}