using Microsoft.AspNetCore.Identity;

namespace Homey.Data.Models;

public class AppUser : IdentityUser
{
    public DateTimeOffset CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTimeOffset? ModifiedOn { get; set; }
    
    public bool IsDisabled { get; set; } = false;
    public SubscriptionLevel SubscriptionLevel { get; set; } = SubscriptionLevel.Basic;
    public IList<Home> Homes { get; set; } = [];
}