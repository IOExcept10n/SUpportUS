using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SupportUS.Web.Controllers;
using SupportUS.Web.Data;
using SupportUS.Web.Models;
using System.Drawing;
using System.Net.Sockets;
using System.Text.Json.Nodes;

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
var controllers = new APIControllers();
// Tasks
app.MapGet("/api/quests/find-by-id", controllers.Quests.FindQuestById);

//Create
app.MapPost("/api/quests/create", controllers.Quests.CreateQuest);

//Update
app.MapPut("/api/quests/update", controllers.Quests.UpdateQuest);

//Delete
app.MapDelete("/api/quests/delete", controllers.Quests.DeleteQuest);

//Take
app.MapPost("/api/quests/take", async (HttpContext context, Guid taskId, Guid workerId) =>
{

});

//Cancel
app.MapPost("/api/quests/cancel", async (HttpContext context, Guid taskId, Guid personId) =>
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
