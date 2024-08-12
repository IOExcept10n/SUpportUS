using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Data;
using System.Net.Sockets;

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

//Create
app.MapPost("/api/reviews/createTaskv", async (HttpContext context, string authorName, string description, int coins) =>
{

});

//Update
app.MapPost("/api/reviews/updateTask", async (HttpContext context, Guid taskId, string description, int coins) =>
{

});

//Delete
app.MapPost("/api/reviews/deleteTask", async (HttpContext context, Guid taskId) =>
{

});

//Take
app.MapPost("/api/reviews/takeTask", async (HttpContext context, Guid taskId, Guid workerId) =>
{

});

//Cancel
app.MapPost("/api/reviews/cancelTask", async (HttpContext context, Guid taskId, Guid personId) =>
{

});

//Report
app.MapPost("/api/reviews/reportTask", async (HttpContext context, Ticket persone, Guid personId) =>
{

});

//Complete
app.MapPost("/api/reviews/completeTask", async (HttpContext context, Guid taskId, Guid personId) =>
{

});

// Reviews
app.MapPost("/api/reviews/publishRewiew", async (HttpContext context, Guid author, string text, int rate) =>
{

});

app.MapPost("/api/reviews/deleteRewiew", async (HttpContext context, Guid idPersone, Guid idReview) =>
{

});

app.MapPost("/api/reviews/reportRewiew", async (HttpContext context, Guid idReport, Ticket ticketPersone) =>
{

});


//Profile
app.MapPost("/api/reviews/createProfile", async (HttpContext context, Guid idReport, Ticket ticketPersone) =>
{

});

app.MapPost("/api/reviews/reportProfile", async (HttpContext context, Guid idAuthor, Guid idReporter, Ticket ticketPersone) =>
{

});

//AdminPanel
app.MapPost("/api/reviews/banAdminPanel", async (HttpContext context, Guid idPersone, Ticket ticketReasone) =>
{

});

app.MapPost("/api/reviews/unBanAdminPanel", async (HttpContext context, Guid idPersone) =>
{

});

app.MapPost("/api/reviews/setCoinsAdminPanel", async (HttpContext context, Guid idPersone, int coins) =>
{

});

app.MapPost("/api/reviews/undoTaskAdminPanel", async (HttpContext context, Guid idTask) =>
{

});




app.Run();
