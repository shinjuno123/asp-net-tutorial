using GameStore.Api.Dtos;

namespace GameStore.Api.Endpoints;

// This is static class
public static class GamesEndpoints
{

    // Static class can have static fields
    private static readonly List<GameDto> games = [
        new GameDto(1, "The Witcher 3: Wild Hunt", "RPG", 39.99m, new DateOnly(2015, 5, 19)),
        new GameDto(2, "Cyberpunk 2077", "Action RPG", 59.99m, new DateOnly(2020, 12, 10)),
        new GameDto(3, "Minecraft", "Sandbox", 26.95m, new DateOnly(2011, 11, 18)),
        new GameDto(4, "Among Us", "Party", 4.99m, new DateOnly(2018, 6, 15)),
        new GameDto(5, "Hades", "Roguelike", 24.99m, new DateOnly(2020, 9, 17))
    ];

    private static readonly string GetGameEndpointName = "GetGame";

    // Whoever calls this method to just chain another call
    // into another extension method that is also extended with webapplication
    // if they want to
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games");
        
        // GET /games
        group.MapGet("/", () => games);

        // GET /games/{id}
        group.MapGet("/{id:int}", (int id) =>
        {
            GameDto? game = games.Find(game => game.Id == id);
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndpointName);

        // POST /games
        group.MapPost("/", (CreateGameDto createGameDto) =>
        {
            GameDto game = new(
                 games.Count + 1,
                 createGameDto.Name,
                 createGameDto.Genre,
                 createGameDto.Price,
                 createGameDto.ReleaseDate
            );

            games.Add(game);

            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
        });

        // Put /games
        group.MapPut("/{id:int}", (int id, UpdateGameDto updatedGame) =>
        {
            var index = games.FindIndex(game => game.Id == id);

            // There are 2 ways of handling this
            // 1. If the index is -1, we return a 404 Not Found response
            // 2. If the index is -1, we create a new game with the provided id and add it to the list
            // What to do here is not clearly defined in the requirements, so I'll go with option 1
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
        });

        group.MapDelete("/{id:int}", (int id) =>
        {
            games.RemoveAll(game => game.Id == id);
            return Results.NoContent();
        });


        return group;
    }

}
