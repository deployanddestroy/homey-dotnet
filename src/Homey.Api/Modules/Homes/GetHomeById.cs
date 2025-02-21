namespace Homey.Api.Modules.Homes;

public class GetHomeById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id}", Handle)
            .WithSummary("Gets a home by Id")
            .WithRequestValidation<Request>();
    }
    
    public record Request(Guid Id);

    public record Response(Guid Id, string Name, string? StreetAddress, string? AddressLine2, string? City, string? PostalCode, string? Country, string? StateProvince);

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request request, 
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        var home = await db.Homes
            .Where(h => h.Id == request.Id && h.UserId == claimsPrincipal.GetUserId())
            .Select(h => new Response(h.Id, h.Name, h.StreetAddress, h.AddressLine2, h.City, h.PostalCode, h.Country, h.StateProvince))
            .SingleOrDefaultAsync(cancellationToken);
        
        return home == null ? TypedResults.NotFound() : TypedResults.Ok(home);
    }
}