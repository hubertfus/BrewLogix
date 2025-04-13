using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BrewLogix.Services
{
    public class KegService
    {
        private readonly AppDbContext _context;

        public KegService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Keg> GetAllKegs()
        {
            return _context.Kegs
                .Include(k => k.Batch)
                .ToList();
        }

        public Keg? GetKegById(int id)
        {
            return _context.Kegs
                .Include(k => k.Batch)
                .FirstOrDefault(k => k.Id == id);
        }

        public void AddKeg(Keg keg)
        {
            keg.FilledAt = keg.FilledAt.ToUniversalTime(); // PostgreSQL wymaga UTC
            _context.Kegs.Add(keg);
            _context.SaveChanges();
        }

        public void UpdateKeg(Keg keg)
        {
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
            var keg = _context.Kegs.Find(id);
            if (keg != null)
            {
                _context.Kegs.Remove(keg);
                _context.SaveChanges();
            }
        }
    }
}