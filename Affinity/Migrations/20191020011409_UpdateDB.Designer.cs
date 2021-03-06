﻿// <auto-generated />
using Affinity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Affinity.Migrations
{
    [DbContext(typeof(AffinityDbcontext))]
    [Migration("20191020011409_UpdateDB")]
    partial class UpdateDB
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Affinity.Models.Edge", b =>
                {
                    b.Property<int>("DBID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Color");

                    b.Property<int>("Direction");

                    b.Property<int>("First");

                    b.Property<string>("GraphID");

                    b.Property<int>("ID");

                    b.Property<string>("Name");

                    b.Property<int>("Second");

                    b.Property<int>("Weight");

                    b.HasKey("DBID");

                    b.ToTable("Edges");
                });

            modelBuilder.Entity("Affinity.Models.User", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GraphID");

                    b.Property<string>("UID");

                    b.HasKey("ID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Affinity.Models.Vertex", b =>
                {
                    b.Property<int>("DBID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Color");

                    b.Property<string>("GraphID");

                    b.Property<int>("ID");

                    b.Property<string>("Name")
                        .HasMaxLength(8);

                    b.Property<int>("XPos");

                    b.Property<int>("YPos");

                    b.HasKey("DBID");

                    b.ToTable("Vertices");
                });
#pragma warning restore 612, 618
        }
    }
}
