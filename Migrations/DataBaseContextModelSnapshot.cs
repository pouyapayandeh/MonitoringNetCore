﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Monitoring.Presistence.Contexts;

#nullable disable

namespace MonitoringNetCore.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    partial class DataBaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("Monitoring.Domain.Entities.Books.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<int>("Number")
                        .HasMaxLength(20)
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("Monitoring.Site.Domain.Entities.VideoFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("VideoFile");
                });
#pragma warning restore 612, 618
        }
    }
}