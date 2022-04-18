using RabbitMQ.Database;
using RabbitMQ.RabbitMQ;
using RabbitMQ.Services;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace RabbitMQ;

public static class RabbitMQServer
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddDbContext<RabbitMQDbContext>((config) =>
        {
            config.UseSqlite(builder.Configuration.GetConnectionString("Database"));
        });

        builder.Services.AddSingleton(
            new ConnectionFactory
            {
                HostName = builder.Configuration.GetConnectionString("MQServer")
            });
        builder.Services.AddSingleton<MqProducer>();
        builder.Services.AddSingleton<MqConsumer>();
        builder.Services.AddHostedService<PostCreationService>();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();
        app.MapControllers();
        app.UseSwagger();
        app.UseSwaggerUI((options) =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        app.Run();
    }
}