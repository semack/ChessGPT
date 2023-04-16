using ChessGPT.Abstract;
using Microsoft.Extensions.Hosting;

namespace ChessGPT.Services;

public class GameService : BackgroundService
{
    private readonly IGame _game;

    public GameService(IGame game)
    {
        _game = game;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        await _game.StartAsync(token);
    }
}