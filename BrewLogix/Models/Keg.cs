using System.ComponentModel.DataAnnotations;

namespace BrewLogix.Models;

public interface IDistributable
{
    bool IsDistributed { get; set; }
    DateTime FilledAt { get; set; }
}

public class Keg : BaseEntity, IDistributable
{
    [Required(ErrorMessage = "Keg code is required")]
    [StringLength(50, ErrorMessage = "Code can't be longer than 50 characters")]
    public string Code { get; set; }
    
    [Required(ErrorMessage = "Batch is required")]
    public int BatchId { get; set; }
    public Batch Batch { get; set; }

    [Required(ErrorMessage = "Size is required")]
    [StringLength(20, ErrorMessage = "Size can't be longer than 20 characters")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Size must be greater than 0")]
    public string Size { get; set; } 
    public bool IsDistributed { get; set; }
    
    [Required(ErrorMessage = "Filled date is required")]
    [DataType(DataType.Date)]
    public DateTime FilledAt { get; set; }

    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}
