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
        public AffinityDbcontext(DbContextOptions<AffinityDbcontext> options):base(options)
        {

        }

        public DbSet<Vertex> Vertices { get; set; }
       
        public DbSet<Edge> Edges { get; set; }

        public DbSet<VertexEdge> VertexEdges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vertex>().ToTable("Vertices");
            modelBuilder.Entity<Edge>().ToTable("Edges");

            modelBuilder.Entity<VertexEdge>().HasKey(ve => new { ve.VertexID, ve.EdgeID });

            modelBuilder.Entity<VertexEdge>()
                .HasOne(v => v.Vertex)
                .WithMany(e => e.VertexEdges)
                .HasForeignKey(ve => ve.VertexID);

            modelBuilder.Entity<VertexEdge>()
                .HasOne(e => e.Edge)
                .WithMany(v => v.VertexEdges)
                .HasForeignKey(ve => ve.EdgeID);
        }
    }
}
