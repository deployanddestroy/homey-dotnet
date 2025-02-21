namespace Homey.Data.Models;

public class InventoryItem : OwnedEntity
{
    public Guid HomeId { get; set; }
    public Guid? RoomId { get; set; }
    
    public required string Name {get; set;}
    public string? Description {get; set;}
    public double? Price {get; set;}
    public string? SerialNumber {get; set;}
    
    public Home Home { get; init; } = null!;
    public Room Room { get; init; } = null!;
}