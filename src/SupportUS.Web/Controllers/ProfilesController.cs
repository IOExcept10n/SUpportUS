﻿using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Data;
using SupportUS.Web.Models;

namespace SupportUS.Web.Controllers
{
    public class ProfilesController(WebApplication app, APIControllers controllers)
        : ControllerBase(app, controllers)
    {
        public async Task GetProfileById(HttpContext context, long id)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var profile = db.Profiles.Find(id);
            var result = new
            {
                Status = profile != null,
                Profile = profile,
            };
            await context.Response.WriteAsJsonAsync(result);
        }

        public async Task CreateProfile(HttpContext context, long id)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            if (await db.Profiles.AnyAsync(x => x.Id == id))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Profile already exists.");
                return;
            }
            var profile = new Profile() { Id = id };
            await db.Profiles.AddAsync(profile);
            await db.SaveChangesAsync();
        }

        public async Task ReportProfile(HttpContext context, long id)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var user = await db.Profiles.FindAsync(id);
            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Profile doesn't exist.");
                return;
            }
            var ticket = await context.Request.ReadFromJsonAsync<Ticket>();
            if (ticket == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Request doesn't have the ticket info.");
                return;
            }
            await db.Tickets.AddAsync(ticket);
            await db.SaveChangesAsync();
        }
    }
}
