﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Repository;

#nullable disable

namespace Repository.Migrations
{
    [DbContext(typeof(FolhaDePontoContext))]
    [Migration("20220729034328_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("Domain.Entities.TimeAllocation", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TimeDuration")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("UserId");

                    b.ToTable("TimeAllocations");
                });

            modelBuilder.Entity("Domain.Entities.TimeMoment", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "DateTime");

                    b.ToTable("TimeMoments");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.TimeAllocation", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.TimeMoment", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });
#pragma warning restore 612, 618
        }
    }
}
