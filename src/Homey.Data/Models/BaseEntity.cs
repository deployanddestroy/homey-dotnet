namespace Homey.Data.Models;

public class BaseEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? ModifiedOn { get; set; }
}

public class OwnedEntity : BaseEntity
{
    public required string UserId { get; set; }
}