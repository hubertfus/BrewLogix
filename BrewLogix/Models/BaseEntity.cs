namespace BrewLogix.Models;

public abstract class BaseEntity : IEntity, IAuditable
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}