using Telegram.Bot.Types;

namespace TodoBot.Models;

public class ToDoModel
{
    public ToDoModel(int id, string content)
    {
        Id = id;
        Content = content;
    }
    
    public int Id { get; set; }

    public string Content { get; set; }

    public static implicit operator ToDoModel(Message message)
    {
        return new ToDoModel(message.MessageId, message.Text ?? string.Empty);
    }

    public override string ToString()
    {
        return $"{Id} {Content}";
    }

    public string ToString(int index)
    {
        return $"#{index} {Id} {Content}";
    }
}