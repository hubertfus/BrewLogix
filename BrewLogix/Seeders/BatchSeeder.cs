using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class BatchSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Batches.Any()) return;

            var batches = new List<Batch>
            {
                new Batch
                {
                    Code = "BATCH001",
                    RecipeId = 1, 
                    StartDate = DateTime.UtcNow.AddDays(-5),
                    EndDate = DateTime.UtcNow,
                    Status = "Completed"
                },
                new Batch
                {
                    Code = "BATCH002",
                    RecipeId = 2, 
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(-5),
                    Status = "Completed"
                },
                new Batch
                {
                    Code = "BATCH003",
                    RecipeId = 3, 
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    EndDate = DateTime.UtcNow,
                    Status = "In Progress"
                }
            };

            context.Batches.AddRange(batches);
            context.SaveChanges();
        }
    }
}