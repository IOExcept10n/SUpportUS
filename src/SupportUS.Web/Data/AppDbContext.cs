using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Models;

namespace SupportUS.Web.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Quest> Quests {  get; set; }

        public DbSet<Review> Reviews { get; set; }

        public string DbPath { get; }

        public AppDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "SUpportUS", "quests.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
