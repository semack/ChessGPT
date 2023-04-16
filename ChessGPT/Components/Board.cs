using ChessGPT.Abstract;

namespace ChessGPT.Components;

public class Board : IBoard
{
    public async Task RenderPgnAsync(string pgn, CancellationToken token = default)
    {
        Console.Clear();
        Console.SetCursorPosition(0, 0);
        var parts = pgn.Split(' ');
        var ranks = parts[0].Split('/');
        Array.Reverse(ranks);

        Console.WriteLine("   a b c d e f g h");
        Console.WriteLine();

        for (var i = 0; i < ranks.Length; i++)
        {
            var rank = ranks[i];
            Console.Write(i + 1 + "  ");

            foreach (var c in rank)
            {
                if (char.IsDigit(c))
                {
                    var emptySquares = int.Parse(c.ToString());
                    for (var j = 0; j < emptySquares; j++)
                    {
                        Console.Write(". ");
                    }
                }
                else
                {
                    Console.Write(c + " ");
                }
            }

            Console.WriteLine(" " + (i + 1));
        }

        Console.WriteLine();
        Console.WriteLine("   a b c d e f g h");
        await Task.CompletedTask;
    }
}