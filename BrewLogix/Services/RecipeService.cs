using BrewLogix.Models;
using System.Collections.Generic;
using System.Linq;

namespace BrewLogix.Services
{
    public class RecipeService
    {
        private readonly List<Recipe> _recipes;
        private readonly IngredientService _ingredientService;

        public RecipeService(IngredientService ingredientService)
        {
            _ingredientService = ingredientService;
            var ingredients = _ingredientService.GetAllIngredients().ToList();

            _recipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Name = "Pale Ale",
                    Style = "Ale",
                    Description = "A refreshing pale ale with a balanced malt and hop profile.",
                    Ingredients = new List<RecipeIngredient>
                    {
                        new RecipeIngredient { Ingredient = ingredients[0], Quantity = 3 },
                        new RecipeIngredient { Ingredient = ingredients[1], Quantity = 50 }
                    }
                },
                new Recipe
                {
                    Id = 2,
                    Name = "IPA",
                    Style = "India Pale Ale",
                    Description = "A hoppy IPA with a strong bitter finish.",
                    Ingredients = new List<RecipeIngredient>
                    {
                        new RecipeIngredient { Ingredient = ingredients[0], Quantity = 5 }, 
                        new RecipeIngredient { Ingredient = ingredients[1], Quantity = 100 } 
                    }
                }
            };
        }

        public IEnumerable<Recipe> GetAllRecipes() => _recipes;

        public Recipe GetRecipeById(int id) => _recipes.FirstOrDefault(r => r.Id == id);

        public void AddRecipe(Recipe recipe)
        {
            recipe.Id = _recipes.Max(r => r.Id) + 1;
            _recipes.Add(recipe);
        }

        public void UpdateRecipe(Recipe recipe)
        {
            var index = _recipes.FindIndex(r => r.Id == recipe.Id);
            if (index != -1)
            {
                _recipes[index] = recipe;
            }
        }

        public void DeleteRecipe(Recipe recipe)
        {
            _recipes.Remove(recipe);
        }

        public void AddIngredientToRecipe(Recipe recipe, RecipeIngredient newIngredient, List<Ingredient> ingredients)
        {
            var ingredient = ingredients.FirstOrDefault(i => i.Id == newIngredient.IngredientId);
            if (ingredient != null)
            {
                recipe.Ingredients.Add(new RecipeIngredient
                {
                    Id = recipe.Ingredients.Count + 1,
                    Ingredient = ingredient,
                    IngredientId = ingredient.Id,
                    Quantity = newIngredient.Quantity
                });
            }
        }

        public void RemoveIngredientFromRecipe(Recipe recipe, RecipeIngredient ingredientToRemove)
        {
            recipe.Ingredients.Remove(ingredientToRemove);
        }
    }
}
