using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Controllers;
using SupportUS.Web.Data;
using SupportUS.Web.Models;
using System.Drawing;
using System.Net.Sockets;
using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<QuestsDb>();
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
var controllers = new APIControllers(app);
// Tasks
app.MapGet("/api/quests/find-by-id", controllers.Quests.FindQuestByIdAsync);

//Create
app.MapPost("/api/quests/create", controllers.Quests.CreateQuestAsync);

//Update
app.MapPut("/api/quests/update", controllers.Quests.UpdateQuestAsync);

//Delete
app.MapDelete("/api/quests/delete", controllers.Quests.DeleteQuestAsync);

//Take
app.MapPost("/api/quests/take", controllers.Quests.TakeQuestAsync);

//Cancel
app.MapPost("/api/quests/cancel", controllers.Quests.CancelQuestAsync);

//Report
app.MapPost("/api/quests/report", () => { });

//Complete
app.MapPost("/api/quests/complete", () => { });

// Reviews
app.MapPost("/api/reviews/publish", controllers.Reviews.PublishReviewAsync);

// Delete
app.MapPost("/api/reviews/deleteRewiew", () => { });

// Report
app.MapPost("/api/reviews/report", controllers.Reviews.ReportReviewAsync);


//Profile
app.MapPost("/api/profiles/create", controllers.Profiles.CreateProfileAsync);

app.MapPost("/api/profiles/reportProfile", controllers.Profiles.ReportProfileAsync);

app.MapGet("/api/profiles/find-by-id", controllers.Profiles.GetProfileByIdAsync);

//AdminPanel
//app.MapPost("/api/reviews/banAdminPanel", async (HttpContext context, Guid idPersone, Ticket ticketReasone) =>
//{

//});

//app.MapPost("/api/reviews/unBanAdminPanel", async (HttpContext context, Guid idPersone) =>
//{

//});

//app.MapPost("/api/reviews/setCoinsAdminPanel", async (HttpContext context, Guid idPersone, int coins) =>
//{

//});

//app.MapPost("/api/reviews/undoTaskAdminPanel", async (HttpContext context, Guid idTask) =>
//{

//});

app.Run();
