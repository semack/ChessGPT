using ChatGPT.Net;
using ChatGPT.Net.DTO.ChatGPT;
using ChessDotNet;
using ChessGPT.Abstract;
using ChessGPT.Components;
using ChessGPT.Services;
using ChessGPT.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChessGPT;

class Program
{
    private static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // Run the host
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddJsonFile("appSettings.json", false, true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<ChatGptSettings>(context.Configuration.GetSection(nameof(ChatGptSettings)));

                services.AddLogging(options => options.ClearProviders()); // stop logging to console
                services.AddSingleton<IGame, Game>();
                services.AddSingleton<IBoard, Board>();
                services.AddSingleton<ChessGame>();
                services.AddSingleton<ChatGpt>(x =>
                {
                    var chatGptSettings = context.Configuration.GetSection(nameof(ChatGptSettings))
                        .Get<ChatGptSettings>()!;

                    return new ChatGpt(chatGptSettings.ApiKey!,
                        new ChatGptOptions
                        {
                            Model = chatGptSettings.Model!,
                            BaseUrl = chatGptSettings.BaseUrl!
                        });
                });
                services.AddHostedService<GameService>();
            });
    }
}