using Microsoft.EntityFrameworkCore;
using SupportUS.Web.Models;

namespace SupportUS.Web.Data
{
    public class QuestsDb : DbContext
    {
        public DbSet<Profile> Profiles { get; set; }

        public DbSet<Quest> Quests { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public string DbPath { get; }

        public QuestsDb()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = Path.Join(path, "SUpportUS", "quests.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.CreatedQuests)
                .WithOne(y => y.Customer);
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.CompletedQuests)
                .WithOne(q => q.Executor);
        }
    }
}
