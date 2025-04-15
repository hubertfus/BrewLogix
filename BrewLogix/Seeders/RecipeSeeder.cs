using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class RecipeSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Recipes.Any()) return;

            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    Name = "Recipe 1",
                    Style = "IPA",
                    Description = "A classic American IPA with hoppy bitterness and citrus aroma."
                },
                new Recipe
                {
                    Name = "Recipe 2",
                    Style = "Stout",
                    Description = "A rich and creamy stout with roasted coffee and chocolate notes."
                },
                new Recipe
                {
                    Name = "Recipe 3",
                    Style = "Lager",
                    Description = "A crisp and refreshing lager with a light malt profile."
                }
            };

            context.Recipes.AddRange(recipes);
            context.SaveChanges();
        }
    }
}