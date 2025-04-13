using BrewLogix.Models;

namespace BrewLogix.Services;
public class IngredientService
{
    private readonly AppDbContext _context;

    public IngredientService(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Ingredient> GetAllIngredients()
    {
        return _context.Ingredients.ToList();
    }

    public void AddIngredient(Ingredient ingredient)
    {
        _context.Ingredients.Add(ingredient);
        _context.SaveChanges();
    }

    public void UpdateIngredient(Ingredient ingredient)
    {
        _context.Ingredients.Update(ingredient);
        _context.SaveChanges();
    }

    public void DeleteIngredient(Ingredient ingredient)
    {
        _context.Ingredients.Remove(ingredient);
        _context.SaveChanges();
    }
}
    