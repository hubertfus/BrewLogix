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
            new Ingredient { Id = 2, Name = "Hops", Type = "Hop", Unit = "g" },
            new Ingredient { Id = 3, Name = "Yeast", Type = "Yeast", Unit = "g" },
            new Ingredient { Id = 4, Name = "Sugar", Type = "Sweetener", Unit = "kg" },
            new Ingredient { Id = 5, Name = "Water", Type = "Liquid", Unit = "L" },
            new Ingredient { Id = 6, Name = "Fruit Essence", Type = "Flavoring", Unit = "ml" },
            new Ingredient { Id = 7, Name = "Oats", Type = "Grain", Unit = "kg" },
            new Ingredient { Id = 8, Name = "Lactose", Type = "Sweetener", Unit = "kg" },
            new Ingredient { Id = 9, Name = "Orange Peel", Type = "Flavoring", Unit = "g" },
            new Ingredient { Id = 10, Name = "Coriander", Type = "Spice", Unit = "g" }
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