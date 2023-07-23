using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TodoBot.Services;

public class BotKeyboardService
{
    private readonly ITelegramBotClient _client;
    private readonly IStringLocalizer<CultureBotMessages> _loc;

    public BotKeyboardService(ITelegramBotClient client, IStringLocalizer<CultureBotMessages> loc)
    {
        _client = client;
        _loc = loc;
    }

    public async Task InitializeKeyboard(Message message, CancellationToken cancellationToken)
    {
        await _client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: _loc["MessageGreetings"],
            cancellationToken: cancellationToken,
            parseMode: ParseMode.MarkdownV2
        );
    }
}