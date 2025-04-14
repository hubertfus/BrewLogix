using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models;

public class Client : BaseEntity
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string ContactEmail { get; set; }
    
    [Required(ErrorMessage = "Address is required")]
    [StringLength(200, ErrorMessage = "Address can't be longer than 200 characters")]
    public string Address { get; set; }

    public ICollection<Order> Orders { get; set; }
}
