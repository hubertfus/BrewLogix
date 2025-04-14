using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BrewLogix.Models;

namespace BrewLogix.Validation
{
    public class UniqueIngredientsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var ingredients = value as ICollection<RecipeIngredient>;

            if (ingredients == null || ingredients.Count == 0)
                return ValidationResult.Success;

            var duplicates = ingredients
                .GroupBy(i => i.IngredientId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                return new ValidationResult("Each ingredient can only be used once in a recipe.");
            }

            return ValidationResult.Success;
        }
    }
}