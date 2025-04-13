namespace BrewLogix.Services;

using Microsoft.EntityFrameworkCore;
using BrewLogix.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Batch> Batches { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Keg> Kegs { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<StockEntry> StockEntries { get; set; }

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

