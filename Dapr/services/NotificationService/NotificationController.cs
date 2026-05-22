using Dapr;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Services;

namespace NotificationService;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly ILogger<NotificationController> _logger;
    private readonly NotificationProcessor _processor;

    public NotificationController(ILogger<NotificationController> logger, NotificationProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    [HttpPost("order-created")]
    public async Task<IActionResult> OnOrderCreated([FromBody] OrderEvent orderEvent)
    {
        _logger.LogInformation("Received OrderCreated event for order {OrderId}", orderEvent.OrderId);
        
        await _processor.ProcessNotification(orderEvent);
        
        return base.Ok();
    }
}

public record OrderEvent(
    string EventType,
    string Source,
    DateTime DataTime,
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount);
