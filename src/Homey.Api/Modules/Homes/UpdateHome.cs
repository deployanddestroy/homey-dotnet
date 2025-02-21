using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Homey.Api.Modules.Homes;

public class UpdateHome : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id}", Handle)
            .WithSummary("Updates a home")
            .WithRequestValidation<Request>();
    }

    public record Request(
        [property: Required(AllowEmptyStrings = false), MaxLength(255)]string Name,
        [property: MaxLength(255)]string? StreetAddress,
        [property: MaxLength(255)]string? AddressLine2,
        [property: MaxLength(100)]string? City,
        [property: MaxLength(100)]string? StateProvince,
        [property: MaxLength(100)]string? Country,
        [property: MaxLength(20)]string? PostalCode
        );

    private static async Task<Results<Ok, NotFound>> Handle(Guid id, Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var home = await db.Homes
            .SingleOrDefaultAsync(
                h => h.Id == id 
                && h.UserId == claimsPrincipal.GetUserId(), 
                cancellationToken);
        if (home is null) return TypedResults.NotFound();

        home.Name = request.Name;
        home.StreetAddress = request.StreetAddress;
        home.AddressLine2 = request.AddressLine2;
        home.City = request.City;
        home.StateProvince = request.StateProvince;
        home.Country = request.Country;
        home.PostalCode = request.PostalCode;
        
        await db.SaveChangesAsync(cancellationToken);
        
        //TODO: send an event out

        return TypedResults.Ok();
    }
}