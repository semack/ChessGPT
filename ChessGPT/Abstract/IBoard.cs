namespace ChessGPT.Abstract;

public interface IBoard
{
    public Task RenderPgnAsync(string pgn, CancellationToken token = default);
}