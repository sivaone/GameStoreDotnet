using System.ComponentModel.DataAnnotations;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.EndPoints;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

// Extension methods
app.MapGameEndpoints();
app.MapGenreEndpoints();
await app.MigrateDbAsync();

app.Run();
