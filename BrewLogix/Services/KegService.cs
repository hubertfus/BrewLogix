using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using BrewLogix.dbhelpers;

namespace BrewLogix.Services
{
    public class KegService
    {
        private readonly IDbContextProvider _dbContextProvider;

        public KegService(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public IEnumerable<Keg> GetAllKegs()
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Kegs
                .Include(k => k.Batch)
                .ToList();
        }
        
        public bool IsBatchAssignedToAnyKeg(int batchId)
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Kegs.Any(k => k.BatchId == batchId);
        }

        public Keg? GetKegById(int id)
        {
            using var _context = _dbContextProvider.GetDbContext();
            return _context.Kegs
                .Include(k => k.Batch)
                .FirstOrDefault(k => k.Id == id);
        }

        public void AddKeg(Keg keg)
        {
            using var _context = _dbContextProvider.GetDbContext();
            keg.FilledAt = keg.FilledAt.ToUniversalTime(); 
            _context.Kegs.Add(keg);
            _context.SaveChanges();
        }

        public void UpdateKeg(Keg keg)
        {
            using var _context = _dbContextProvider.GetDbContext();
            var existingKeg = _context.Kegs.Find(keg.Id);
            if (existingKeg != null)
            {
                existingKeg.Code = keg.Code;
                existingKeg.Size = keg.Size;
                existingKeg.IsDistributed = keg.IsDistributed;
                existingKeg.FilledAt = keg.FilledAt.ToUniversalTime();
                existingKeg.BatchId = keg.BatchId;

                _context.SaveChanges();
            }
        }

        public void DeleteKeg(int id)
        {
            using var _context = _dbContextProvider.GetDbContext();
            var keg = _context.Kegs
                .Include(k => k.Order)
                .FirstOrDefault(k => k.Id == id);

            if (keg != null)
            {
                if (keg.Order != null)
                {
                    throw new InvalidOperationException("Cannot delete keg assigned to an order.");
                }

                _context.Kegs.Remove(keg);
                _context.SaveChanges();
            }
        }
    }
}
