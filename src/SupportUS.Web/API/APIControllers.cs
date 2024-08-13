namespace SupportUS.Web.API
{
    public class APIControllers
    {
        public QuestsController Quests { get; }

        public ProfilesController Profiles { get; }

        public ReviewsController Reviews { get; }

        public APIControllers(WebApplication app)
        {
            Quests = new QuestsController(app, this);
            Profiles = new ProfilesController(app, this);
            Reviews = new ReviewsController(app, this);
        }
    }
}
