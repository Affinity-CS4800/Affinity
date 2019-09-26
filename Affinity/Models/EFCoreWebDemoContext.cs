using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Affinity.Models
{
    public class EFCoreWebDemoContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //needs to add a data base for code to work
            optionsBuilder.UseSqlServer(@"Server=.\;Database=INSERT DATABASE HERE;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
