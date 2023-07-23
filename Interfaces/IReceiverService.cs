namespace TodoBot.Interfaces;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}