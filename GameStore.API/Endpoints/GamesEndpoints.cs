using System;
using GameStore.API.Data;
using GameStore.API.Dtos;
using GameStore.API.Entities;
using GameStore.API.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGameById";

    private static readonly List<GameSummaryDto> games =
    [
        new(1, "The Last of Us Part II", "Action-adventure", 59.99m, new DateOnly(2020, 6, 19)),
        new(2, "Ghost of Tsushima", "Action-adventure", 59.99m, new DateOnly(2020, 7, 17)),
        new(3, "Cyberpunk 2077", "Action role-playing", 59.99m, new DateOnly(2020, 12, 10)),
        new(4, "Demon's Souls", "Action role-playing", 69.99m, new DateOnly(2020, 11, 12)),
        new(
            5,
            "Demon Slayer: Kimetsu no Yaiba – Hinokami Keppūtan",
            "Fighting",
            59.99m,
            new DateOnly(2021, 10, 15)
        ),
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games").WithParameterValidation();
        // GET /games
        group.MapGet(
            "/",
            (GameStoreContext dbContext) =>
                dbContext
                    .Games.Include(game => game.Genre)
                    .Select(game => game.ToGameSummaryDto())
                    .AsNoTracking()
        );

        // GET /games/{id}
        group
            .MapGet(
                "/{id}",
                (int id, GameStoreContext dbContext) =>
                {
                    Game? game = dbContext.Games.Find(id);

                    return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
                }
            )
            .WithName(GetGameEndPointName);

        // POST /games
        group.MapPost(
            "/",
            (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                Game game = newGame.ToEntity();

                dbContext.Games.Add(game);
                dbContext.SaveChanges();

                return Results.CreatedAtRoute(
                    GetGameEndPointName,
                    new { id = game.Id },
                    game.ToGameDetailsDto()
                );
            }
        );

        // PUT /games/{id}
        group.MapPut(
            "/{id}",
            (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var existingGame = dbContext.Games.Find(id);

                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));

                dbContext.SaveChanges();

                return Results.NoContent();
            }
        );

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            (int id, GameStoreContext dbContext) =>
            {
                dbContext.Games.Where(game => game.Id == id).ExecuteDelete();

                return Results.NoContent();
            }
        );

        // group.MapGet("/", () => "Hello World!");

        return group;
    }
}
