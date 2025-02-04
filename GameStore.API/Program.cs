using GameStore.API.Dtos;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const string GetGameEndPointName = "GetGameById";

List<GameDto> games = [
  new (1, "The Last of Us Part II", "Action-adventure", 59.99m, new DateOnly(2020, 6, 19)),
  new (2, "Ghost of Tsushima", "Action-adventure", 59.99m, new DateOnly(2020, 7, 17)),
  new (3, "Cyberpunk 2077", "Action role-playing", 59.99m, new DateOnly(2020, 12, 10)),
  new (4, "Demon's Souls", "Action role-playing", 69.99m, new DateOnly(2020, 11, 12)),
  new (5, "Demon Slayer: Kimetsu no Yaiba – Hinokami Keppūtan", "Fighting", 59.99m, new DateOnly(2021, 10, 15))
];

// GET /games
app.MapGet("/games", () => games);

// GET /games/{id}
app.MapGet("/games/{id}", (int id) => games.Find(game => game.Id == id)).WithName(GetGameEndPointName);

// POST /games
app.MapPost("/games", (CreateGameDto newGame) => {
  GameDto game = new(games.Count + 1, newGame.Name, newGame.Genre, newGame.Price, newGame.ReleaseDate);
  games.Add(game);
  return Results.CreatedAtRoute(GetGameEndPointName, new { id = game.Id }, game);
});

// PUT /games/{id}
app.MapPut("/games/{id}", (int id, UpdateGameDto updatedGame) => {
  var index = games.FindIndex(game => game.Id == id);

  games[index] = new GameDto(id, updatedGame.Name, updatedGame.Genre, updatedGame.Price, updatedGame.ReleaseDate);

  return Results.NoContent();
});

// DELETE /games/{id}
app.MapDelete("/games/{id}", (int id) => {
  var index = games.FindIndex(game => game.Id == id);

  games.RemoveAt(index);

  return Results.NoContent();
});

app.MapGet("/", () => "Hello World!");

app.Run();
