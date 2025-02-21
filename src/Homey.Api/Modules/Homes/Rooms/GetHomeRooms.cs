using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Homey.Api.Modules.Homes.Rooms;

public class GetHomeRooms : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{homeId}/rooms", Handle)
            .WithSummary("Get all rooms belonging to a home")
            .WithRequestValidation<Request>();
    }
    
    public record Request(
        [property: Range(1, int.MaxValue)]int? Page, 
        [property: Range(1, 100)]int? PageSize) : IPagedRequest;

    public record Response(Guid Id, Guid HomeId, string Name);

    public static async Task<PagedList<Response>> Handle(
        Guid homeId,
        [AsParameters] Request request,
        AppDbContext db, 
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken)
    {
        return await db.Rooms
            .Where(r => r.HomeId == homeId && r.UserId == claimsPrincipal.GetUserId())
            .OrderBy(r => r.Name)
            .Select(r => new Response(r.Id, r.HomeId, r.Name))
            .ToPagedListAsync(request, cancellationToken);
    }
}