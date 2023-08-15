using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MonitoringNetCore.Domain.Entities;

namespace MonitoringNetCore.Persistence.Contexts
{
    public class DataBaseContext:IdentityDbContext<IdentityUser>
    {
        public DataBaseContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<IdentityUser>().HasKey(iul => iul.Id);
            // modelBuilder.Entity<IdentityUserLogin<>>().HasKey(iul => iul.Id);
        }
        
        public void Migrate()
        {
            Console.WriteLine(Database.ProviderName);
            this.Database.Migrate();
        }
        public DbSet<Camera> Camera { get; set; } = null!;
        public DbSet<VideoFile> VideoFile { get; set; } = null!;
        public DbSet<ProcessLog> ProcessLogs { get; set; } = null!;
        public DbSet<VideoProcessJob> VideoProcessJob { get; set; } = null!;
        public DbSet<License> Licenses { get; set; } = null!;
    }
}
