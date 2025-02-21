using System.ComponentModel.DataAnnotations;

namespace Homey.Data.Models;

public class Home : OwnedEntity
{
    // Core Properties
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public required string Name { get; set; }
    public string? PostalCode { get; set; }
    public string? StreetAddress { get; set; }
    public string? StateProvince { get; set; }
    
    // Relationships
    public AppUser User { get; init; } = null!;
    public IList<Room> Rooms { get; init; } = [];
    public IList<InventoryItem> InventoryItems { get; init; } = [];
}