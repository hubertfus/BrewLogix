using BrewLogix.Models;
using BrewLogix.Services;

namespace BrewLogix.Seeders
{
    public static class IngredientSeeder
    {
        public static void Seed(AppDbContext context)
        {
            if (context.Ingredients.Any()) return;

            var ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Name = "Hops",
                    Type = "Dry",
                    Unit = "grams"
                },
                new Ingredient
                {
                    Name = "Barley",
                    Type = "Grain",
                    Unit = "kilograms"
                },
                new Ingredient
                {
                    Name = "Yeast",
                    Type = "Liquid",
                    Unit = "liters"
                }
            };

            context.Ingredients.AddRange(ingredients);
            context.SaveChanges();
        }
    }
}