using System;
using GameStore.API.Dtos;

namespace GameStore.API.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndPointName = "GetGameById";

    private static readonly List<GameDto> games =
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
        group.MapGet("/", () => games);

        // GET /games/{id}
        group
            .MapGet(
                "/{id}",
                (int id) =>
                {
                    GameDto? game = games.Find(game => game.Id == id);

                    return game is null ? Results.NotFound() : Results.Ok(game);
                }
            )
            .WithName(GetGameEndPointName);

        // POST /games
        group.MapPost(
            "/",
            (CreateGameDto newGame) =>
            {
                GameDto game = new(
                    games.Count + 1,
                    newGame.Name,
                    newGame.Genre,
                    newGame.Price,
                    newGame.ReleaseDate
                );
                games.Add(game);
                return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
            }
        );

        // PUT /games/{id}
        group.MapPut(
            "/{id}",
            (int id, UpdateGameDto updatedGame) =>
            {
                var index = games.FindIndex(game => game.Id == id);

                if (index == -1)
                {
                    return Results.NotFound();
                }

                games[index] = new GameDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                );

                return Results.NoContent();
            }
        );

        // DELETE /games/{id}
        group.MapDelete(
            "/{id}",
            (int id) =>
            {
                var index = games.FindIndex(game => game.Id == id);

                games.RemoveAt(index);

                return Results.NoContent();
            }
        );

        // group.MapGet("/", () => "Hello World!");

        return group;
    }
}
