using BrewLogix.Models;

namespace BrewLogix.Services;

public class IngredientService
{
    private readonly List<Ingredient> _ingredients;

    public IngredientService()
    {
        _ingredients = new List<Ingredient>
        {
            new Ingredient { Id = 1, Name = "Barley", Type = "Grain", Unit = "kg" },
            new Ingredient { Id = 2, Name = "Hops", Type = "Hop", Unit = "g" }
        };
    }

    public IEnumerable<Ingredient> GetAllIngredients() => _ingredients;

    public void AddIngredient(Ingredient ingredient)
    {
        ingredient.Id = _ingredients.Max(i => i.Id) + 1;
        _ingredients.Add(ingredient);
    }
    
    public void UpdateIngredient(Ingredient ingredient)
    {
        var index = _ingredients.FindIndex(i => i.Id == ingredient.Id);
        _ingredients[index] = ingredient;
    }

    public void DeleteIngredient(Ingredient ingredient)
    {
        _ingredients.Remove(ingredient);
    }
}