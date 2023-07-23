using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TodoBot.Services;

public class UpdateHandler : IUpdateHandler
{
    private ITelegramBotClient _client;
    private readonly ILogger _logger;
    
    private readonly TodoActionsService _todoActionsService;
    private readonly BotActionsService _botActionsService;

    public UpdateHandler(ITelegramBotClient client, ILogger<UpdateHandler> logger, 
        TodoActionsService todoActionsService, 
        BotActionsService botActionsService)
    {
        _client = client;
        _logger = logger;
        _todoActionsService = todoActionsService;
        _botActionsService = botActionsService;
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient _, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceive(message, cancellationToken),
            { EditedMessage: { } message } => BotOnEditedMessageReceive(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            { InlineQuery: { } inlineQuery } => BotOnInlineQueryReceived(inlineQuery, cancellationToken),
            { ChosenInlineResult: { } chosenInlineResult } => BotOnChoiceResultReceived(chosenInlineResult, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceive(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message: {MessageType} {MessageId}", message.Type, message.MessageId);

        if (message.Text is null)
        {
            await _client.DeleteMessageAsync(
                chatId: message.Chat.Id,
                messageId: message.MessageId,
                cancellationToken: cancellationToken);
            return;
        }

        var action = message.Text.Split(' ')[0] switch
        {
            BotActions.Start => _botActionsService.StartAction(message, cancellationToken),
            // TODO Добавить переключение тудушек
            _ => _todoActionsService.CreateTodo(message, cancellationToken)
        };
        await action;
    }

    private async Task BotOnEditedMessageReceive(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Edit message: {MessageId}", message.MessageId);
        await _todoActionsService.EditTodo(message, cancellationToken);
    }

    private async Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
    }

    private async Task BotOnChoiceResultReceived(ChosenInlineResult chosenInlineResult,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message: {MessageType}", chosenInlineResult.Query);
    }

    private async Task BotOnInlineQueryReceived(InlineQuery inlineQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline query from: {MessageType}", inlineQuery.Query);
    }

    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        
        if (callbackQuery.Message is not { } message) return;
        
        var action = callbackQuery.Data switch
        {
            CallbackQueries.TodoComplete => _todoActionsService.CompleteTodo(message, cancellationToken),
            CallbackQueries.TodoRemove => _todoActionsService.RemoveTodo(message, cancellationToken),
            _ => OutOfRangeAction(message, cancellationToken)
        };

        try
        {
            await action;
        }
        catch
        {
            await OutOfRangeAction(message, cancellationToken);
        }
    }

    private async Task OutOfRangeAction(Message message, CancellationToken cancellationToken)
    {
        await _client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: "Error",
            cancellationToken: cancellationToken
        );
    }
}