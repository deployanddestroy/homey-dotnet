using Asp.Versioning;
using Homey.Api.Modules.Homes;
using Homey.Api.Modules.Homes.Rooms;
using Homey.Api.Modules.Identity;
using Homey.Api.Modules.Inventory;
using Homey.Api.Modules.Pros;

namespace Homey.Api.Modules;

public static class Endpoints
{
    /// <summary>
    /// Maps all the endpoints in the application.
    /// </summary>
    /// <param name="app">WebApplication object</param>
    /// <returns>WebApplication object with mapped endpoints</returns>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // Set up API versioning
        var apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();
        
        // Create a versioned group using the set above
        var versionedRouteGroup = app.MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);
        
        // Map all the endpoints
        MapHomeEndpoints(versionedRouteGroup);
        MapIdentityEndpoints(versionedRouteGroup);
        MapProfessionalEndpoints(versionedRouteGroup);
        MapInventoryEndpoints(versionedRouteGroup);
        return app;
    }
    
    /// <summary>
    /// Maps all the home-related endpoints. Includes room endpoints,
    /// since they are children of the home entity.
    /// </summary>
    /// <param name="routeGroupBuilder">Route builder object</param>
    private static void MapHomeEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var endpoints = routeGroupBuilder
            .MapGroup("/homes")
            .WithTags("Homes");

        endpoints.RequireAuthorization()
            // Homes
            .MapEndpoint<GetHomeById>()
            .MapEndpoint<GetHomes>()
            .MapEndpoint<AddHome>()
            .MapEndpoint<UpdateHome>()
            .MapEndpoint<DeleteHome>()
            // Rooms
            .MapEndpoint<GetHomeRooms>()
            .MapEndpoint<AddRoom>()
            .MapEndpoint<DeleteRoom>()
            .MapEndpoint<UpdateRoom>();
    }
    
    /// <summary>
    /// Maps all the endpoints related to professionals.
    /// </summary>
    /// <param name="routeGroupBuilder">Route builder object</param>
    private static void MapProfessionalEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var endpoints = routeGroupBuilder
            .MapGroup("/pros")
            .WithTags("Professionals");

        endpoints.RequireAuthorization()
            .MapEndpoint<GetPros>()
            .MapEndpoint<AddPro>()
            .MapEndpoint<UpdatePro>()
            .MapEndpoint<DeletePro>()
            .MapEndpoint<GetProTypes>();
    }

    /// <summary>
    /// Maps all the endpoints related to inventory. Items must belong to a home,
    /// but they may or may not belong to a room.
    /// </summary>
    /// <param name="routeGroupBuilder">Route builder object</param>
    private static void MapInventoryEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var endpoints = routeGroupBuilder
            .MapGroup("/inventory")
            .WithTags("Inventory");

        endpoints.RequireAuthorization()
            .MapEndpoint<GetInventoryItems>()
            .MapEndpoint<AddInventoryItem>()
            .MapEndpoint<UpdateInventoryItem>()
            .MapEndpoint<DeleteInventoryItem>()
            .MapEndpoint<GetInventoryItemById>();
    }
    
    /// <summary>
    /// Maps all the identity endpoints. Code is cloned from ASP.NET Identity's repo,
    /// but can now be re-mapped with a route group.
    /// </summary>
    /// <param name="routeGroupBuilder"></param>
    private static void MapIdentityEndpoints(this RouteGroupBuilder routeGroupBuilder)
    {
        var endpoints = routeGroupBuilder
            .MapGroup("/auth")
            .WithTags("Authentication");
        
        endpoints.MapCustomIdentityApi<AppUser>();
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder routeBuilder)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(routeBuilder);
        return routeBuilder;
    }
}