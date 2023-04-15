using System.Globalization;
using ChatGPT.Net;
using ChessDotNet;
using ChessGPT.Abstract;
using ChessGPT.Enums;
using ChessGPT.Settings;
using Microsoft.Extensions.Options;
using File = System.IO.File;

namespace ChessGPT.Components;

public class Game : IGame
{
    private readonly IBoard _board;
    private readonly ChessGame _chessGame;
    private readonly ChatGpt _chatGpt;
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
                await MakeMoveAsync(answer);
                await InvalidateBoardAsync(token);
                await Task.Delay(15000, token);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task MakeMoveAsync(string answer)
    {
        var positions = answer.Split('-');
        var move = new Move(positions[0], positions[1], _chessGame.WhoseTurn);
        _chessGame.MakeMove(move, true);
        await Task.CompletedTask;
    }
    
    private async Task<string> GetMoveFromChatAsync(ChessGame chessGame)
    {
        var filePath = Path.Combine(_settings.RequestTemplate);
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