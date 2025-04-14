using BrewLogix.dbhelpers;
using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;

namespace BrewLogix.Services;

public class StockEntriesService
{
    private readonly IDbContextProvider _dbContextProvider;

    public StockEntriesService(IDbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }

    public IEnumerable<StockEntry> GetAllStockEntries()
    {
        using var _context = _dbContextProvider.GetDbContext();
        return _context.StockEntries.Include(se => se.Ingredient).ToList();
    }

    public IEnumerable<StockEntry> GetStockEntriesForIngredient(int ingredientId)
    {
        using var _context = _dbContextProvider.GetDbContext();
        return _context.StockEntries
            .Include(se => se.Ingredient)
            .Where(se => se.IngredientId == ingredientId)
            .ToList();
    }

    public void AddStockEntry(StockEntry stockEntry)
    {
        using var _context = _dbContextProvider.GetDbContext();
        _context.StockEntries.Add(stockEntry);
        _context.SaveChanges();
    }

    public void UpdateStockEntry(StockEntry stockEntry)
    {
        using var _context = _dbContextProvider.GetDbContext();
        var trackedEntity = _context.StockEntries.Local.FirstOrDefault(e => e.Id == stockEntry.Id);
        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }
        _context.StockEntries.Update(stockEntry);
        _context.SaveChanges();
    }

    public StockEntry GetStockEntry(int id)
    {
        using var _context = _dbContextProvider.GetDbContext();
        return _context.StockEntries.Include(se => se.Ingredient).FirstOrDefault(se => se.Id == id);
    }

    public void DeleteStockEntry(StockEntry stockEntry)
    {
        using var _context = _dbContextProvider.GetDbContext();
        _context.StockEntries.Remove(stockEntry);
        _context.SaveChanges();
    }
}