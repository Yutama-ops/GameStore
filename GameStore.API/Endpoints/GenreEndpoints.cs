using System;
using GameStore.API.Data;
using GameStore.API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Endpoints;

public static class GenreEndpoints
{
    const string GetGenreEndPointName = "GetGenreById";

    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("genres").WithParameterValidation();

        group.MapGet(
            "/",
            async (GameStoreContext dbContext) =>
                await dbContext.Genres.Select(genre => genre.ToDto()).AsNoTracking().ToListAsync()
        );

        return group;
    }
}
