using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Monitoring.Domain.Entities.Books;

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
    }
}
