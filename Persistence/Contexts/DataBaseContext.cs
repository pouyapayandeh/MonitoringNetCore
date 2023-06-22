using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Monitoring.Domain.Entities.Books;
using Monitoring.Site.Domain.Entities;

namespace Monitoring.Presistence.Contexts
{
    public class DataBaseContext:DbContext, IDataBaseContext
    {
        public DataBaseContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Book> Books { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            
        }

        public DbSet<Monitoring.Site.Domain.Entities.VideoFile> VideoFile { get; set; }
        public DbSet<Monitoring.Site.Domain.Entities.Camera> Camera { get; set; }
    }
}
