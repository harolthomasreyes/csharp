using Dapr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _dbContext;

    public OrdersController(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = request.CustomerName,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }
}

public record CreateOrderRequest(
    [property: Required] string CustomerName,
    [property: Required] decimal TotalAmount);
