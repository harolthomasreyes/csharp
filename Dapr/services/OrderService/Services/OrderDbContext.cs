using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Services;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
}
