using System;
using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GenreEndpoints
{
    public static void MapGenreEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("/genres");

        group.MapGet("", async (GameStoreContext dbContext) =>
            await dbContext.Genres.AsNoTracking().ToListAsync()
        );
    }
}
