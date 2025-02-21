namespace Homey.Api.Modules.Pros;

public class GetProTypes : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/types", Handle)
            .WithSummary("Gets list of pro types")
            .CacheOutput(p => p
                .Expire(TimeSpan.FromDays(1))
                .Tag(WellKnownKeys.ProTypes));
    }
    
    public record Response(Guid Id, string Name);

    private static IEnumerable<Response> Handle(AppDbContext db)
    {
        return db.ProfessionalTypes
            .Select(p => new Response(p.Id, p.Name));
    }
}