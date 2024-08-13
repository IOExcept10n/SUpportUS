using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Data;
using SupportUS.Web.Models;

namespace SupportUS.Web.API
{
    public class ProfilesController(WebApplication app, APIControllers controllers)
        : ControllerBase(app, controllers)
    {
        public async Task GetProfileByIdAsync(HttpContext context, long id)
        {
            using var db = GetDbContext();
            var profile = db.Profiles.Find(id);
            var result = new
            {
                Status = profile != null,
                Profile = profile,
            };
            await context.Response.WriteAsJsonAsync(result);
        }

        public async Task CreateProfileAsync(HttpContext context, long id)
        {
            using var db = GetDbContext();
            if (await db.Profiles.AnyAsync(x => x.Id == id))
            {
                await TellClientErrorAsync(context, "Profile already exists.");
                return;
            }
            var profile = new Profile() { Id = id };
            await db.Profiles.AddAsync(profile);
            await db.SaveChangesAsync();
        }

        public async Task ReportProfileAsync(HttpContext context, long id)
        {
            using var db = Application.Services.GetRequiredService<QuestsDb>();
            var user = await db.Profiles.FindAsync(id);
            if (user == null)
            {
                await TellNotFoundAsync(context, nameof(user));
                return;
            }
            var ticket = await context.Request.ReadFromJsonAsync<Ticket>();
            if (ticket == null)
            {
                await TellClientErrorAsync(context, "Request doesn't have the ticket info.", StatusCodes.Status400BadRequest);
                return;
            }
            await db.Tickets.AddAsync(ticket);
            await db.SaveChangesAsync();
        }
    }
}
