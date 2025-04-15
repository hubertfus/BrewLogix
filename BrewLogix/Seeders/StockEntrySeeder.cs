using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class StockEntrySeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.StockEntries.Any()) return;

            var stockEntries = new List<StockEntry>
            {
                new StockEntry
                {
                    IngredientId = 1, 
                    Quantity = 100.5m,
                    DeliveryDate = DateTime.UtcNow.AddDays(-5),
                    ExpiryDate = DateTime.UtcNow.AddMonths(6)
                },
                new StockEntry
                {
                    IngredientId = 2, 
                    Quantity = 200.3m,
                    DeliveryDate = DateTime.UtcNow.AddDays(-10),
                    ExpiryDate = DateTime.UtcNow.AddMonths(6)
                }
            };

            context.StockEntries.AddRange(stockEntries);
            context.SaveChanges();
        }
    }
}