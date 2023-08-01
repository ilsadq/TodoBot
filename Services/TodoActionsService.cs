using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TodoBot.Services;

public class TodoActionsService
{
    private readonly ITelegramBotClient _client;
    
    public TodoActionsService(ITelegramBotClient client)
    {
        _client = client;
    }

    public async Task CreateTodo(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null) return;
        
        await _client.DeleteMessageAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            cancellationToken: cancellationToken);
        
        await _client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: message.Text,
            parseMode: ParseMode.Markdown,
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("✅", CallbackQueries.TodoComplete),
                InlineKeyboardButton.WithCallbackData("❌", CallbackQueries.TodoRemove)
            }),
            cancellationToken: cancellationToken);
    }

    public async Task EditTodo(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is null) return;
        
        await _client.DeleteMessageAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            cancellationToken: cancellationToken);
        
        await _client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: $"✏️ {message.Text}",
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }

    public async Task CompleteTodo(Message message, CancellationToken cancellationToken)
    {
        await _client.DeleteMessageAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            cancellationToken: cancellationToken);
    }

    public async Task RemoveTodo(Message message, CancellationToken cancellationToken)
    {
        await _client.DeleteMessageAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            cancellationToken: cancellationToken);
    }
}
