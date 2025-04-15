using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class RecipeIngredientSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.RecipeIngredients.Any()) return;

            var recipeIngredients = new List<RecipeIngredient>
            {
                new RecipeIngredient
                {
                    RecipeId = 1,
                    IngredientId = 1, 
                    Quantity = 50.0m,
                    SelectedStockEntryId = 1 
                },
                new RecipeIngredient
                {
                    RecipeId = 1, 
                    IngredientId = 2, 
                    Quantity = 100.0m,
                    SelectedStockEntryId = 2 
                },
                new RecipeIngredient
                {
                    RecipeId = 2,
                    IngredientId = 3,
                    Quantity = 20.0m,
                    SelectedStockEntryId = 3 
                }
            };

            context.RecipeIngredients.AddRange(recipeIngredients);
            context.SaveChanges();
        }
    }
}