    using BrewLogix.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using BrewLogix.dbhelpers;

    namespace BrewLogix.Services
    {
        public class RecipeService
        {
            private readonly IDbContextProvider _dbContextProvider;

            public RecipeService(IDbContextProvider dbContextProvider)
            {
                _dbContextProvider = dbContextProvider;
            }

            public IEnumerable<Recipe> GetAllRecipes()
            {
                using var _context = _dbContextProvider.GetDbContext();
                return _context.Recipes
                    .Include(r => r.Ingredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .ToList();
            }

            public Recipe GetRecipeById(int id)
            {
                using var _context = _dbContextProvider.GetDbContext();
                return _context.Recipes
                    .Include(r => r.Ingredients)
                        .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefault(r => r.Id == id);
            }

            public void AddRecipe(Recipe recipe)
            {
                using var _context = _dbContextProvider.GetDbContext();

                foreach (var ri in recipe.Ingredients)
                {
                    if (ri.Ingredient == null)
                    {
                        ri.Ingredient = new Ingredient { Id = ri.IngredientId };
                    }

                    _context.Attach(ri.Ingredient);
                }

                _context.Recipes.Add(recipe);
                _context.SaveChanges();
            }


            public void UpdateRecipe(Recipe updatedRecipe)
            {
                using var _context = _dbContextProvider.GetDbContext();

                var existingRecipe = _context.Recipes
                    .Include(r => r.Ingredients)
                    .FirstOrDefault(r => r.Id == updatedRecipe.Id);

                if (existingRecipe == null)
                    throw new InvalidOperationException("Recipe not found.");

                // Update basic fields
                existingRecipe.Name = updatedRecipe.Name;
                existingRecipe.Style = updatedRecipe.Style;
                existingRecipe.Description = updatedRecipe.Description;

                // Synchronize ingredients
                var updatedIngredientIds = updatedRecipe.Ingredients.Select(i => i.Id).ToList();

                // Remove missing ones
                var toRemove = existingRecipe.Ingredients
                    .Where(i => !updatedIngredientIds.Contains(i.Id))
                    .ToList();

                foreach (var ri in toRemove)
                {
                    _context.Remove(ri);
                }

                // Update existing or add new ones
                foreach (var updated in updatedRecipe.Ingredients)
                {
                    var existing = existingRecipe.Ingredients.FirstOrDefault(i => i.Id == updated.Id);

                    if (existing != null)
                    {
                        existing.Quantity = updated.Quantity;
                        existing.IngredientId = updated.IngredientId;
                    }
                    else
                    {
                        existingRecipe.Ingredients.Add(new RecipeIngredient
                        {
                            IngredientId = updated.IngredientId,
                            Quantity = updated.Quantity
                        });
                    }
                }

                _context.SaveChanges();
            }



            public void DeleteRecipe(Recipe recipe)
            {
                using var _context = _dbContextProvider.GetDbContext();

                bool isUsedSomewhere = _context.Batches.Any(b => b.RecipeId == recipe.Id);

                if (isUsedSomewhere)
                {
                    throw new InvalidOperationException("Cannot delete recipe. It is currently in use by a batch.");
                }

                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
            }

            public void AddIngredientToRecipe(Recipe recipe, RecipeIngredient newIngredient)
            {
                using var _context = _dbContextProvider.GetDbContext();
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
                using var _context = _dbContextProvider.GetDbContext();
                recipe.Ingredients.Remove(ingredientToRemove);
                _context.SaveChanges();
            }
        }
    }
