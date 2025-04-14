    using System.ComponentModel.DataAnnotations;

    namespace BrewLogix.Models;

    public class Batch : BaseEntity
    {
        [Required(ErrorMessage = "Code is required")]
        [StringLength(50, ErrorMessage = "Code can't be longer than 50 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Recipe is required")]
        public int RecipeId { get; set; }
        
        
        
        public Recipe Recipe { get; set; }
        
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(30, ErrorMessage = "Status can't be longer than 30 characters")]
        public string Status { get; set; } 
        
        public ICollection<Keg> Kegs { get; set; }
        public ICollection<StockEntry> StockEntries { get; set; }

    }
