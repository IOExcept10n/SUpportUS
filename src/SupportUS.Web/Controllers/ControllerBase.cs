using SupportUS.Web.Data;

namespace SupportUS.Web.Controllers
{
    public class ControllerBase(WebApplication app, APIControllers controllers)
    {
        public WebApplication Application { get; } = app;

        public APIControllers Controllers { get; } = controllers;
    }
}
