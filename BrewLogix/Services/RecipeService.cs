using BrewLogix.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BrewLogix.Services
{
    public class RecipeService
    {
        private readonly AppDbContext _context;

        public RecipeService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Recipe> GetAllRecipes()
        {
            return _context.Recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                .ToList();
        }

        public Recipe GetRecipeById(int id)
        {
            return _context.Recipes
                .Include(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefault(r => r.Id == id);
        }

        public void AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            _context.SaveChanges();
        }

        public void UpdateRecipe(Recipe recipe)
        {
            var tracked = _context.Recipes.Local.FirstOrDefault(r => r.Id == recipe.Id);
            if (tracked != null)
                _context.Entry(tracked).State = EntityState.Detached;

            _context.Recipes.Update(recipe);
            _context.SaveChanges();
        }

        public void DeleteRecipe(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
            _context.SaveChanges();
        }

        public void AddIngredientToRecipe(Recipe recipe, RecipeIngredient newIngredient)
        {
            recipe.Ingredients ??= new List<RecipeIngredient>();

            var ingredient = _context.Ingredients.FirstOrDefault(i => i.Id == newIngredient.IngredientId);
            if (ingredient != null)
            {
                newIngredient.Ingredient = ingredient;
                recipe.Ingredients.Add(newIngredient);

                _context.Recipes.Update(recipe);
                _context.SaveChanges();
            }
        }

        public void RemoveIngredientFromRecipe(Recipe recipe, RecipeIngredient ingredientToRemove)
        {
            recipe.Ingredients.Remove(ingredientToRemove);
            _context.SaveChanges();
        }
    }
}
