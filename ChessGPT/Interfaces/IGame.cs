namespace ChessGPT.Interfaces;

public interface IGame
{
    public Task StartAsync(CancellationToken token);
}