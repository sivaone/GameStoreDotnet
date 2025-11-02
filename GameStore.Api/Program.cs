using System.ComponentModel.DataAnnotations;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.EndPoints;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

List<GameDto> games = new()
{
    new (1, "Skyward Quest", "Adventure", 59.99m, new DateOnly(2023, 5, 12)),
    new (2, "Nebula Racer", "Racing", 39.99m, new DateOnly(2022, 11, 2)),
    new (3, "Forge & Fortify", "Strategy", 49.99m, new DateOnly(2024, 2, 28))
};

// Route groups
// var group = app.MapGroup("/games");
// group.MapGet("/", () => games);

// GET /games
app.MapGet("/games", () => games);

const string GetGameEndpointName = "GetGame";

// GET /games/1
app.MapGet("/games/{id}", (int id) =>
{
    GameDto? game = games.Find(game => game.Id == id);

    return game is null ? Results.NotFound() : Results.Ok(game);
})
.WithName(GetGameEndpointName);

// POST /games
app.MapPost("/games", (BaseGameDto newGame) =>
{
    // Below is not working with records
    /* var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(newGame);
    if(!Validator.TryValidateObject(newGame, context, validationResults, true))
    {
        return Results.ValidationProblem(validationResults);
    } */

    // MiniValidation package
    if(!MiniValidator.TryValidate(newGame, out var validationResults))
    {
        return Results.ValidationProblem(validationResults); // standard structure
        // Results.BadRequest(validationResults); // custom structure
    }

    GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate);
    games.Add(game);

    return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
});

// PUT /games/1
app.MapPut("/games/{id}", (int id, BaseGameDto updateGame) =>
{
    var index = games.FindIndex(game => game.Id == id);
    // Either return not found or create a resource
    if (index == -1)
    {
        return Results.NotFound();
    }

    games[index] = new(
        id,
        updateGame.Name,
        updateGame.Genre,
        updateGame.Price,
        updateGame.ReleaseDate);


    return Results.NoContent();
});

// DELETE /games/1
app.MapDelete("/games/{id}", (int id) =>
{
    games.RemoveAll(game => game.Id == id);

    return Results.NoContent();

});

// Extension methods
app.MapGameEndpoints();

app.Run();
