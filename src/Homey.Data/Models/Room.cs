namespace Homey.Data.Models;

public class Room : OwnedEntity
{
    public required string Name { get; set; }
    
    public required Guid HomeId { get; set; }
    public Home Home { get; init; } = null!;
    public IList<InventoryItem> InventoryItems { get; init; } = [];
}