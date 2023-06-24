using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Monitoring.Site.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Monitoring.Presistence.Contexts
{
    public class DataBaseContext:IdentityDbContext<IdentityUser>
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {
            // Database

        }

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

        public DbSet<Monitoring.Site.Domain.Entities.VideoFile> VideoFile { get; set; }
        
        public DbSet<Monitoring.Site.Domain.Entities.Camera> Camera { get; set; }
    }
}
