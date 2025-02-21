using System.ComponentModel.DataAnnotations;

namespace Homey.Data.Models;

public class Professional : OwnedEntity
{
    public string? BusinessName { get; set; }
    public string? BusinessPhoneNumber { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? EmailAddress { get; set; }
    public required string Name { get; set; }
    public string? PersonalPhoneNumber { get; set; }
    public string? PostalCode { get; set; }
    public Guid ProfessionalTypeId { get; set; }
    public string? StateProvince { get; set; }
    public string? StreetAddress { get; set; }
    public string? WebsiteUrl { get; set; }
    
    // Relationships
    public ProfessionalType ProfessionalType { get; init; } = null!;
    public AppUser User { get; init; } = null!;
}

public class ProfessionalType
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}