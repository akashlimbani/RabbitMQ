using RabbitMQ.Models;
using Microsoft.EntityFrameworkCore;

namespace RabbitMQ.Database;

public class RabbitMQDbContext : DbContext
{
    public RabbitMQDbContext(DbContextOptions<RabbitMQDbContext> options) : base(options) {}
    
    public DbSet<Post> Posts { get; set; }
}