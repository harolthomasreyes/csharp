namespace NotificationService.Services;

public class NotificationProcessor
{
    private readonly ILogger<NotificationProcessor> _logger;

    public NotificationProcessor(ILogger<NotificationProcessor> logger)
    {
        _logger = logger;
    }

    public Task ProcessNotification(OrderEvent orderEvent)
    {
        _logger.LogInformation("Processing notification: Order {OrderId} created by {CustomerName} - Total: {TotalAmount:C}",
            orderEvent.OrderId, orderEvent.CustomerName, orderEvent.TotalAmount);

        return Task.CompletedTask;
    }
}
