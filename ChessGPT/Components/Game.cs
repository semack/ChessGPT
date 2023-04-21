using ChatGPT.Net;
using ChessDotNet;
using ChessGPT.Interfaces;
using ChessGPT.Settings;
using Microsoft.Extensions.Options;
using File = System.IO.File;

namespace ChessGPT.Components;

public class Game : IGame
{
    private readonly IBoard _board;
    private readonly ChatGpt _chatGpt;
    private readonly ChessGame _chessGame;
    private readonly ChatGptSettings _settings;

    public Game(IBoard board, ChessGame chessGame, ChatGpt chatGpt, IOptions<ChatGptSettings> options)
    {
        _board = board;
        _chessGame = chessGame;
        _chatGpt = chatGpt;
        _settings = options.Value;
    }

    public async Task StartAsync(CancellationToken token)
    {
        await InvalidateBoardAsync(token);
        try
        {
            while (!token.IsCancellationRequested)
            {
                var answer = await GetMoveFromChatAsync(_chessGame);
                var gameAlive = await MakeMoveAsync(answer);
                await InvalidateBoardAsync(token);
                Console.WriteLine($"\nLast move: {answer}");
                if (!gameAlive)
                    break;
                await Task.Delay(15000, token); //delay for the calling api
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<bool> MakeMoveAsync(string answer)
    {
        var positions = answer.Split('-');
        var move = new Move(positions[0], positions[1], _chessGame.WhoseTurn);
        _chessGame.MakeMove(move, true);
        var gameAlive = !_chessGame.IsCheckmated(_chessGame.WhoseTurn) &&
                        !_chessGame.IsStalemated(_chessGame.WhoseTurn);
        return await Task.FromResult(gameAlive);
    }

    private async Task<string> GetMoveFromChatAsync(ChessGame chessGame)
    {
        var filePath = Path.Combine(_settings.RequestTemplate!);
        var template = await File.ReadAllTextAsync(filePath);
        var question = string.Format(template, chessGame.GetFen());
        var answer = await _chatGpt.Ask(question);
        return answer;
    }

    private async Task InvalidateBoardAsync(CancellationToken token)
    {
        var pgn = _chessGame.GetFen();
        await _board.RenderPgnAsync(pgn, token);
    }
}