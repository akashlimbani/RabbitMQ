using System.Text;
using System.Text.Json;
using RabbitMQ.Database;
using RabbitMQ.Models;
using RabbitMQ.RabbitMQ;
using Microsoft.EntityFrameworkCore;

namespace RabbitMQ.Services;

public class PostCreationService : BackgroundService
{
    readonly IServiceScopeFactory _factory;
    readonly MqConsumer _consumer;
    
    public PostCreationService(IServiceScopeFactory factory, MqConsumer consumer)
    {
        _factory = factory;
        _consumer = consumer;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.AddListener(nameof(Post),  (_, args) =>
        {
            using var scope = _factory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RabbitMQDbContext>();
            var body = args.Body.ToArray();
            var postJson = Encoding.UTF8.GetString(body);
            var post = JsonSerializer.Deserialize<Post>(postJson)!;
            dbContext.Add(post);
            dbContext.SaveChanges();
        });
        
        return Task.CompletedTask;
    }
}