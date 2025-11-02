using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mappers;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

namespace GameStore.Api.EndPoints;

public static class GameEndpointExtensions
{
    const string GetGameEndpointName = "GetGame";

    public static void MapGameEndpoints(this WebApplication app)
    {
        app.MapGet("/hello", () => "Hello world!");

        // Route groups
        // var group = app.MapGroup("/games");
        // group.MapGet("/", () => games);

        // GET /games
        app.MapGet("/games", async (GameStoreContext dbContext) =>
        {
            return await dbContext.Games
                .Include(game => game.Genre)
                .Select(game => game.ToDto())
                .AsNoTracking()
                .ToListAsync();
        });


        // GET /games/1
        app.MapGet("/games/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(g => g.Id == id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToDto());
        })
        .WithName(GetGameEndpointName);


        // POST /games
        app.MapPost("/games", async (BaseGameDto newGame, GameStoreContext dbContext) =>
        {

            // MiniValidation package
            if (!MiniValidator.TryValidate(newGame, out var validationResults))
            {
                return Results.ValidationProblem(validationResults); // standard structure
                // Results.BadRequest(validationResults); // custom structure
            }

            Game game = newGame.ToEntity();
            // game.Genre = dbContext.Genres.Find(newGame.GenreId);

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            await dbContext.Entry(game).Reference(g => g.Genre).LoadAsync();

            GameDto gameDto = game.ToDto();

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDto.Id }, gameDto);
        });


        // PUT /games/1
        app.MapPut("/games/{id}", async (int id, BaseGameDto updateGame, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);
            // Either return not found or create a resource
            if (game is null)
            {
                return Results.NotFound();
            }

            game.Name = updateGame.Name;
            game.GenreId = updateGame.GenreId;
            // game.Genre = dbContext.Genres.Find(updateGame.GenreId);
            game.Price = updateGame.Price;
            game.ReleaseDate = updateGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });
        

        // DELETE /games/1
        app.MapDelete("/games/{id}", async (int id, GameStoreContext dbContext) =>
        {
            // Method 1: Batch delete
            await dbContext.Games
                .Where(g => g.Id == id)
                .ExecuteDeleteAsync();

            // Method 2: Remove() - SaveChanges() IS needed
            /* var game = await dbContext.Games.FindAsync(id);
            if (game is not null)
            {
                dbContext.Games.Remove(game);
                await dbContext.SaveChangesAsync();
            } */

            return Results.NoContent();
        });

    }

}
