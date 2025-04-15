using BrewLogix.dbhelpers;
using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;

namespace BrewLogix.Services
{
    public class BatchService
    {
        private readonly IDbContextProvider _dbContextProvider;

        public BatchService(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public List<Batch> GetAllBatches()
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Batches
                .Include(b => b.Recipe)
                .Include(b => b.Kegs)
                .Include(b => b.StockEntries)
                    .ThenInclude(se => se.Ingredient)
                .ToList();
        }

        public Batch GetBatchById(int id)
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Batches
                .Include(b => b.Recipe)
                .Include(b => b.Kegs)
                .Include(b => b.StockEntries)
                    .ThenInclude(se => se.Ingredient)
                .FirstOrDefault(b => b.Id == id);
        }

        public void AddBatch(Batch batch)
        {
            using var _context = _dbContextProvider.GetDbContext();

            if (batch.Recipe != null)
            {
                var existingRecipe = _context.Recipes.Find(batch.Recipe.Id);
                if (existingRecipe != null)
                {
                    batch.Recipe = existingRecipe;
                }
                else
                {
                    throw new Exception("Recipe not found");
                }
            }

            _context.Batches.Add(batch);
            _context.SaveChanges();
        }


        public void UpdateBatch(Batch batch)
        {
            using var _context = _dbContextProvider.GetDbContext();
            var existing = _context.Batches
                .Include(b => b.Kegs)
                .Include(b => b.StockEntries)
                .FirstOrDefault(b => b.Id == batch.Id);

            if (existing != null)
            {
                existing.Code = batch.Code;
                existing.StartDate = batch.StartDate;
                existing.Status = batch.Status;
                _context.SaveChanges();
            }
        }

        public void DeleteBatch(Batch batch)
        {
            using var _context = _dbContextProvider.GetDbContext();
            _context.Batches.Remove(batch);
            _context.SaveChanges();
        }
    }
}
