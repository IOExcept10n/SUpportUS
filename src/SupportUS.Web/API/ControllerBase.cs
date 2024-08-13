using SupportUS.Web.Data;

namespace SupportUS.Web.API
{
    public class ControllerBase(WebApplication app, APIControllers controllers)
    {
        public WebApplication Application { get; } = app;

        public APIControllers Controllers { get; } = controllers;

        public async Task TellNotFoundAsync(HttpContext context, string name) 
            => await TellClientErrorAsync(context, $"Required {name} not found.");

        public async Task TellClientErrorAsync(HttpContext context, string message, int statusCode = StatusCodes.Status403Forbidden)
        {
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new
            {
                Message = message,
            });
        }

        protected QuestsDb GetDbContext()
        {
            return Application.Services.GetRequiredService<QuestsDb>();
        }
    }
}
