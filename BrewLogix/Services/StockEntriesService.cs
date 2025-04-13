using BrewLogix.Models;
using BrewLogix.Services;
using Microsoft.EntityFrameworkCore;

namespace BrewLogix.Services;

public class StockEntriesService
{
    private readonly AppDbContext _context;

    public StockEntriesService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<StockEntry> GetAllStockEntries()
    {
        return _context.StockEntries.Include(se => se.Ingredient).ToList();
    }

    public IEnumerable<StockEntry> GetStockEntriesForIngredient(int ingredientId)
    {
        return _context.StockEntries
            .Include(se => se.Ingredient)
            .Where(se => se.IngredientId == ingredientId)
            .ToList();
    }

    public void AddStockEntry(StockEntry stockEntry)
    {
        _context.StockEntries.Add(stockEntry);
        _context.SaveChanges();
    }

    public void UpdateStockEntry(StockEntry stockEntry)
    {
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
        return _context.StockEntries.Include(se => se.Ingredient).FirstOrDefault(se => se.Id == id);
    }

    public void DeleteStockEntry(StockEntry stockEntry)
    {
        _context.StockEntries.Remove(stockEntry);
        _context.SaveChanges();
    }
}