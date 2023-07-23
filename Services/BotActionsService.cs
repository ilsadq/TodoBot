using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TodoBot.Services;

public class BotActionsService
{
    private readonly ITelegramBotClient _client;
    private readonly BotKeyboardService _keyboardService;
    
    public BotActionsService(ITelegramBotClient client, BotKeyboardService keyboardService)
    {
        _client = client;
        _keyboardService = keyboardService;
    }
    
    public async Task StartAction(Message message, CancellationToken cancellationToken)
    {
        await _client.DeleteMessageAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            cancellationToken: cancellationToken);
        await _keyboardService.InitializeKeyboard(message, cancellationToken);
    }
}