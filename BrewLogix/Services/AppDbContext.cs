namespace BrewLogix.Services
{
    using Microsoft.EntityFrameworkCore;
    using BrewLogix.Models;

    public class AppDbContext : DbContext
    {
        private readonly string _connectionString;

        // Konstruktor DbContext
        public AppDbContext(DbContextOptions<AppDbContext> options, string connectionString = null) : base(options)
        {
            // Ustawienie ConnectionString na podstawie przekazanej wartości (lub domyślnej)
            _connectionString = connectionString;
        }

        public DbSet<Batch> Batches { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Keg> Kegs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<StockEntry> StockEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                optionsBuilder.UseNpgsql(_connectionString); 
            }
            else
            {
                base.OnConfiguring(optionsBuilder); 
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Recipe)
                .WithMany(r => r.Ingredients)
                .HasForeignKey(ri => ri.RecipeId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(ri => ri.Ingredient)
                .WithMany(i => i.UsedInRecipes)
                .HasForeignKey(ri => ri.IngredientId);
        }
    }
}
