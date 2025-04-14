using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models;

public class Order : BaseEntity
{
    [Required(ErrorMessage = "Client is required")]
    public int ClientId { get; set; }
    public Client Client { get; set; }
    
    [Required(ErrorMessage = "Order date is required")]
    [DataType(DataType.Date)]
    public DateTime OrderedAt { get; set; }
    [Required(ErrorMessage = "Status is required")]
    [StringLength(30, ErrorMessage = "Status can't be longer than 30 characters")]
    public string Status { get; set; } 

    public ICollection<Keg> Kegs { get; set; }
}
