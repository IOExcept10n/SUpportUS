using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<AppDbContext>();
var app = builder.Build();

app.MapGet("/", async context =>
{
    var response = context.Response;
    response.ContentType = "text/html; charset=utf-8";
    await response.WriteAsync("<h2>Cookies: </h2>");
    foreach (var item in context.Request.Cookies)
    {
        await response.WriteAsync($"<h4>[{item.Key}] = {item.Value}</h4>");
    }
});

// API

// Tasks
app.MapGet("/api/tasks/find-task-by-id", async (HttpContext context, Guid id) =>
{
    var db = app.Services.GetRequiredService<AppDbContext>();
    var task = await db.Quests.FindAsync(id);
    var result = new
    {
        Found = task != null,
        Task = task
    };
    await context.Response.WriteAsJsonAsync(result);
});

// Reviews
app.MapPost("/api/reviews/publish", async (HttpContext context, Guid author, string text, int rate) =>
{

});

app.Run();
