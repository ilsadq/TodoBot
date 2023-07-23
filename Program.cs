using TodoBot.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<BotConfiguration>(
            context.Configuration.GetSection(BotConfiguration.Configuration));

        services.AddHttpClient("telegram_bot_client")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                var config = sp.GetConfiguration<BotConfiguration>();
                var options = new TelegramBotClientOptions(config.BotToken);
                return new TelegramBotClient(options, httpClient);
            });

        services.AddLocalization();
        
        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
        services.AddHostedService<PollingService>();

        services.AddScoped<TodoActionsService>();
        services.AddScoped<BotActionsService>();
        services.AddScoped<BotKeyboardService>();
    })
    .Build();

host.Run();

internal class BotConfiguration
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; set; } = string.Empty;
}
