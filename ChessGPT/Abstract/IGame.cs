namespace ChessGPT.Abstract;

public interface IGame
{
    public Task StartAsync(CancellationToken token);
}