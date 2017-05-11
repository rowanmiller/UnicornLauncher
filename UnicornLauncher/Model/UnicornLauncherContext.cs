using Microsoft.EntityFrameworkCore;

namespace UnicornLauncher.Model
{
    public class UnicornLauncherContext : DbContext
    {
        public DbSet<Launch> Launches { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=launchdata.db");
        }
    }
}
