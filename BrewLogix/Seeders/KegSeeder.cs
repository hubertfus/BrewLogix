using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class KegSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Kegs.Any()) return;

            var kegs = new List<Keg>
            {
                new Keg
                {
                    Code = "KEG001",
                    BatchId = 1,
                    Size = "50L",
                    IsDistributed = false,
                    FilledAt = DateTime.UtcNow.AddDays(-2)
                },
                new Keg
                {
                    Code = "KEG002",
                    BatchId = 2,
                    Size = "30L",
                    IsDistributed = false,
                    FilledAt = DateTime.UtcNow.AddDays(-1)
                },
                new Keg
                {
                    Code = "KEG003",
                    BatchId = 3,
                    Size = "20L",
                    IsDistributed = false,
                    FilledAt = DateTime.UtcNow.AddDays(-3)
                }
            };

            context.Kegs.AddRange(kegs);
            context.SaveChanges();
        }
    }
}