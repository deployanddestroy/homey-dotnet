using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Homes;

public class GetHomes : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle)
            .WithSummary("Gets homes for the current user")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Range(1, int.MaxValue)]int? Page, 
        [property: Range(1, 100)]int? PageSize) : IPagedRequest;

    public record Response(
        Guid Id, 
        string Name, 
        string? StreetAddress, 
        string? AddressLine2, 
        string? City, 
        string? PostalCode, 
        string? Country, 
        string? StateProvince);

    private static async Task<PagedList<Response>> Handle(
        [AsParameters] Request request, 
        ClaimsPrincipal claimsPrincipal, 
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        return await db.Homes
            .Where(h => h.UserId == claimsPrincipal.GetUserId())
            .OrderBy(h => h.Name)
            .Select(h => new Response(
                h.Id, 
                h.Name, 
                h.StreetAddress, 
                h.AddressLine2, 
                h.City, 
                h.PostalCode, 
                h.Country, 
                h.StateProvince))
            .ToPagedListAsync(request, cancellationToken);
    }
}