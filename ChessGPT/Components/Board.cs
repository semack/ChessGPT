using ChessGPT.Abstract;

namespace ChessGPT.Components;

public class Board : IBoard
{
    public async Task RenderPgnAsync(string pgn, CancellationToken token = default)
    {
        Console.Clear();
        Console.SetCursorPosition(0,0);
        string[] parts = pgn.Split(' ');
        string[] ranks = parts[0].Split('/');
        Array.Reverse(ranks);

        Console.WriteLine("   a b c d e f g h");
        Console.WriteLine();

        for (int i = 0; i < ranks.Length; i++)
        {
            string rank = ranks[i];
            Console.Write((i + 1) + "  ");

            foreach (char c in rank)
            {
                if (Char.IsDigit(c))
                {
                    int emptySquares = int.Parse(c.ToString());
                    for (int j = 0; j < emptySquares; j++)
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
