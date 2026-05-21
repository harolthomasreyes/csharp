using Dapr;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Services;

namespace NotificationService;

public class NotificationController
{
    private readonly ILogger<NotificationController> _logger;
    private readonly NotificationProcessor _processor;

    public NotificationController(ILogger<NotificationController> logger, NotificationProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    [Topic("pubsub", "order-events",
        Rules = new[]
        {
            new TopicRule
            {
                Match = "event.type == 'OrderCreated'",
                Path = "/api/notification/order-created"
            }
        })]
    [HttpPost("api/notification/order-created")]
    public async Task<IActionResult> OnOrderCreated([FromBody] OrderEvent orderEvent)
    {
        _logger.LogInformation("Received OrderCreated event for order {OrderId}", orderEvent.OrderId);
        
        await _processor.ProcessNotification(orderEvent);
        
        return Ok();
    }
}

public record OrderEvent(
    string EventType,
    string Source,
    DateTime DataTime,
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount);
