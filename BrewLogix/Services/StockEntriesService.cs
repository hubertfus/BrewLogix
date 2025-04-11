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
            new StockEntry { Id = 1, Ingredient = ingredients[0], Quantity = 50, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(6) },
            new StockEntry { Id = 2, Ingredient = ingredients[1], Quantity = 200, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3) },
            new StockEntry { Id = 2, Ingredient = ingredients[0], Quantity = 2137, DeliveryDate = DateTime.Now, ExpiryDate = DateTime.Now.AddMonths(3) }
        };
    }
    
    public IEnumerable<StockEntry> GetAllStockEntries() => _stockEntries;

    
    public IEnumerable<StockEntry> GetStockEntriesForIngredient(int ingredientId)
    {
        return _stockEntries.Where(se => se.IngredientId == ingredientId);
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

    public void DeleteStockEntry(StockEntry stockEntry)
    {
        _stockEntries.Remove(stockEntry);
    }
}