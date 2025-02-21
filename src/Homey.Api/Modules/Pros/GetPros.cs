using System.ComponentModel.DataAnnotations;

namespace Homey.Api.Modules.Pros;

public class GetPros : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", Handle)
            .WithSummary("Gets pros for the current user")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Range(1, int.MaxValue)]int? Page, 
        [property: Range(1, 100)]int? PageSize) : IPagedRequest;

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
        ProfessionalType Type);
    
    private static async Task<PagedList<Response>> Handle(
        [AsParameters] Request request, 
        ClaimsPrincipal claimsPrincipal, 
        AppDbContext db,
        CancellationToken cancellationToken)
    {
        return await db.Professionals
            .Where(h => h.UserId == claimsPrincipal.GetUserId())
            .Select(h => new Response(
                h.Id, 
                h.BusinessName,
                h.BusinessPhoneNumber,
                h.EmailAddress,
                h.Name, 
                h.PersonalPhoneNumber,
                h.PostalCode,
                h.City,
                h.Country,
                h.StateProvince,
                h.StreetAddress, 
                h.WebsiteUrl, 
                h.ProfessionalType))
            .ToPagedListAsync(request, cancellationToken);
    }
}