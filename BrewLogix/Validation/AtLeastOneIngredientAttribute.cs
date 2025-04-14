using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Validation
{
    public class AtLeastOneIngredientAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var list = value as ICollection;
            return list != null && list.Count > 0;
        }
    }
}