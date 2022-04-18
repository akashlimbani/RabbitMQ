using System.Text;
using System.Text.Json;
using RabbitMQ.Database;
using RabbitMQ.Models;
using RabbitMQ.RabbitMQ;
using Microsoft.AspNetCore.Mvc;

namespace RabbitMQ.Controllers;

[ApiController]
[Route("/api/posts")]
public class PostController : ControllerBase
{
    readonly RabbitMQDbContext _context;
    readonly MqProducer _producer;

    public PostController(RabbitMQDbContext context, MqProducer producer)
    {
        _context = context;
        _producer = producer;
    }

    [HttpGet]
    public List<Post> GetPosts()
    {
       return _context.Posts.ToList();
    }

    [HttpPost]
    public Post CreatePost([FromBody] Post post)
    {
        _context.Add(post);
        _context.SaveChanges();
        return post;
    }

    [HttpPost("/async")]
    public void CreatePostAsync([FromBody] Post post)
    {
        var jsonText = JsonSerializer.Serialize(post);
        var encodedText = Encoding.UTF8.GetBytes(jsonText);
        _producer.SendMessage(nameof(Post), encodedText);
    }
}