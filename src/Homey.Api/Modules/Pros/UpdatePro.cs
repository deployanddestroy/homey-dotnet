using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.OutputCaching;

namespace Homey.Api.Modules.Pros;

public class UpdatePro : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id}", Handle)
            .WithSummary("Updates a pro")
            .WithRequestValidation<Request>();
    }

    public record Request(
        [property: MaxLength(255)]string? BusinessName,
        [property: MaxLength(50)]string? BusinessPhoneNumber,
        [property: MaxLength(100)]string? EmailAddress,
        [property: Required(AllowEmptyStrings = false), MaxLength(255)]string Name,
        [property: MaxLength(50)]string? PersonalPhoneNumber,
        [property: MaxLength(20)]string? PostalCode,
        [property: MaxLength(100)]string? City,
        [property: MaxLength(100)]string? Country,
        [property: MaxLength(100)]string? StateProvince,
        [property: MaxLength(255)]string? StreetAddress,
        [property: MaxLength(255)]string? WebsiteUrl);
    
    public record Response(
        Guid Id, 
        string? BusinessName,
        string? BusinessPhoneNumber,
        string? EmailAddress,
        string Name, 
        string? PersonalPhoneNumber, 
        string? PostalCode,
        string? City, 
        string? Country, 
        string? StateProvince,
        string? StreetAddress,
        string? WebsiteUrl,
        ProfessionalType? Type);
    
    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        Guid id,
        Request request, 
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var pro = await db.Professionals
            .SingleOrDefaultAsync(
                h => h.Id == id 
                && h.UserId == claimsPrincipal.GetUserId(), 
                cancellationToken);
        if (pro is null) return TypedResults.NotFound();
        
        pro.BusinessName = request.BusinessName;
        pro.BusinessPhoneNumber = request.BusinessPhoneNumber;
        pro.EmailAddress = request.EmailAddress;
        pro.Name = request.Name;
        pro.PersonalPhoneNumber = request.PersonalPhoneNumber;
        pro.PostalCode = request.PostalCode;
        pro.City = request.City;
        pro.Country = request.Country;
        pro.StateProvince = request.StateProvince;
        pro.StreetAddress = request.StreetAddress;
        pro.WebsiteUrl = request.WebsiteUrl;
        
        await db.SaveChangesAsync(cancellationToken);
        
        // fetch the pro type
        var proType = await db.ProfessionalTypes.Where(p => p.Id == pro.ProfessionalTypeId).SingleOrDefaultAsync();
        
        var response = new Response(
            pro.Id,  
            pro.BusinessName,
            pro.BusinessPhoneNumber,
            pro.EmailAddress,
            pro.Name, 
            pro.PersonalPhoneNumber,
            pro.PostalCode,
            pro.City,
            pro.Country,
            pro.StateProvince,
            pro.StreetAddress, 
            pro.WebsiteUrl,
            proType);
        return TypedResults.Ok(response);
    }
}