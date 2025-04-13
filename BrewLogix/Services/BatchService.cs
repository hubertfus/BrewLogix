using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;

namespace BrewLogix.Services
{
    public class BatchService
    {
        private readonly AppDbContext _context;

        public BatchService(AppDbContext context)
        {
            _context = context;
        }

        public List<Batch> GetAllBatches()
        {
            return _context.Batches
                .Include(b => b.Recipe)
                .Include(b => b.Kegs)
                .Include(b => b.StockEntries)
                .ThenInclude(se => se.Ingredient)
                .ToList();
        }

        public Batch GetBatchById(int id)
        {
            return _context.Batches
                .Include(b => b.Recipe)
                .Include(b => b.Kegs)
                .Include(b => b.StockEntries)
                .ThenInclude(se => se.Ingredient)
                .FirstOrDefault(b => b.Id == id);
        }

        public void AddBatch(Batch batch)
        {
            _context.Batches.Add(batch);
            _context.SaveChanges();
        }

        public void UpdateBatch(Batch batch)
        {
            var existing = _context.Batches.Include(b => b.Kegs).Include(b => b.StockEntries).FirstOrDefault(b => b.Id == batch.Id);

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
            _context.Batches.Remove(batch);
            _context.SaveChanges();
        }
    }
}