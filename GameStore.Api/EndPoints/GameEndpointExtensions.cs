using System;

namespace GameStore.Api.EndPoints;

public static class GameEndpointExtensions
{

    public static void MapGameEndpoints(this WebApplication app)
    {
        app.MapGet("/hello", () => "Hello world!");
    }

}
