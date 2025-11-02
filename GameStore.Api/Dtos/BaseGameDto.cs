using System;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class BaseGameDto(
    [Required][StringLength(50)]string Name,
    int GenreId,
    [Range(0.99,99.99)]decimal Price,
    DateOnly ReleaseDate
);
