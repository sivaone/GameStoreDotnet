using System;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mappers;

public static class GameMapper
{
    public static Game ToEntity(this BaseGameDto gameDto)
    {
        Game game = new()
        {
            Name = gameDto.Name,
            GenreId = gameDto.GenreId,
            Price = gameDto.Price,
            ReleaseDate = gameDto.ReleaseDate
        };

        return game;
    }

    public static Game UpdateEntity(this BaseGameDto gameDto, Game game)
    {
        game.Name = gameDto.Name;
        game.GenreId = gameDto.GenreId;
        game.Price = gameDto.Price;
        game.ReleaseDate = gameDto.ReleaseDate;

        return game;
    }

    public static GameDto ToDto(this Game game)
    {
        GameDto gameDto = new(
                    game.Id,
                    game.Name,
                    game.Genre!.Name,
                    game.Price,
                    game.ReleaseDate
                );

        return gameDto;
    }
}
