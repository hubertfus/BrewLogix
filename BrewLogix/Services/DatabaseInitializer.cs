using BrewLogix.Seeders;
using BrewLogix.Services;

namespace BrewLogix
{
    public static class DatabaseInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            ClientSeeder.Seed(context);
            IngredientSeeder.Seed(context);
            RecipeSeeder.Seed(context);
            RecipeIngredientSeeder.Seed(context);
            StockEntrySeeder.Seed(context);
            BatchSeeder.Seed(context);
            KegSeeder.Seed(context);
            OrderSeeder.Seed(context);
        }
    }
}