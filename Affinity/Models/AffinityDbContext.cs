using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Affinity.Models;

namespace Affinity.Models
{
    public class AffinityDbcontext : DbContext
    {
        public AffinityDbcontext(DbContextOptions<AffinityDbcontext> options) : base(options) {}

        public DbSet<Vertex> Vertices { get; set; }
        public DbSet<Edge> Edges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vertex>().ToTable("Vertices");
            modelBuilder.Entity<Edge>().ToTable("Edges");
        }
    }
}
