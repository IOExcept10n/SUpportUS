using SupportUS.Web.Data;
using SupportUS.Web.Models;
using System.Net.Sockets;

namespace SupportUS.Web.Controllers
{
    public class ProfilesController(WebApplication app, APIControllers controllers)
        : ControllerBase(app, controllers)
    {
        public async Task CreateProfile(HttpContext context, long Id)
        {
            using var db = Application.Services.GetRequiredService<AppDbContext>();
            if(db.Profiles.FindAsync(Id)  == null) 
            {
                var profile = new Profile() { Id = Id };
                await db.Profiles.AddAsync(profile);
                await db.SaveChangesAsync();
            }
            else
            {
                await context.Response.WriteAsync("Profile is already existe");
                return;
            }
        }
    }
}
