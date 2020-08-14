﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RplpAvecBD.Data;

namespace RplpAvecBD.Migrations
{
    [DbContext(typeof(RplpContext))]
    partial class RplpContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("RplpAvecBD.Model.Professeur", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("apiKey")
                        .HasColumnType("nvarchar(40)")
                        .HasMaxLength(40);

                    b.Property<string>("courriel")
                        .IsRequired()
                        .HasColumnType("nvarchar(45)")
                        .HasMaxLength(45);

                    b.Property<string>("nom")
                        .HasColumnType("nvarchar(45)")
                        .HasMaxLength(45);

                    b.HasKey("id");

                    b.ToTable("Professeur");
                });
#pragma warning restore 612, 618
        }
    }
}