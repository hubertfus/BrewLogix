using BrewLogix.dbhelpers;
using BrewLogix.Models;

namespace BrewLogix.Services;

public class IngredientService
{
    private readonly IDbContextProvider _dbContextProvider;

    public IngredientService(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public IEnumerable<Ingredient> GetAllIngredients()
    {
        using var _context = _dbContextProvider.GetDbContext();
        return _context.Ingredients.ToList();
    }

    public void AddIngredient(Ingredient ingredient)
    {
        using var _context = _dbContextProvider.GetDbContext();
        _context.Ingredients.Add(ingredient);
        _context.SaveChanges();
    }

    public void UpdateIngredient(Ingredient ingredient)
    {
        using var _context = _dbContextProvider.GetDbContext();
        _context.Ingredients.Update(ingredient);
        _context.SaveChanges();
    }

    public void DeleteIngredient(Ingredient ingredient)
    {
        using var _context = _dbContextProvider.GetDbContext();

        bool isUsedInStockEntries = _context.StockEntries.Any(se => se.IngredientId == ingredient.Id);

        if (isUsedInStockEntries)
        {
            throw new InvalidOperationException("Cannot delete ingredient. It is used in one or more stock entries.");
        }

        _context.Ingredients.Remove(ingredient);
        _context.SaveChanges();
    }


}