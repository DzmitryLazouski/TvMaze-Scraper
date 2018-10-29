﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Scraper;
using Scraper.Contexts;


namespace Scraper.Migrations
{
    [DbContext(typeof(ShowsContext))]
    [Migration("20181026094312_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Scraper.Person", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Birthday");

                    b.Property<string>("Name");

                    b.Property<int>("ShowId");

                    b.HasKey("Id");

                    b.HasIndex("ShowId");

                    b.ToTable("Actor");
                });

            modelBuilder.Entity("Scraper.Show", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Show");
                });

            modelBuilder.Entity("Scraper.Person", b =>
                {
                    b.HasOne("Scraper.Show", "Show")
                        .WithMany("Cast")
                        .HasForeignKey("ShowId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
