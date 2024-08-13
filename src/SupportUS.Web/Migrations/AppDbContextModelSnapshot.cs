﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SupportUS.Web.Data;

#nullable disable

namespace SupportUS.Web.Migrations
{
    [DbContext(typeof(QuestsDb))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("SupportUS.Web.Models.Profile", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Coins")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Rating")
                        .HasColumnType("REAL");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("SupportUS.Web.Models.Quest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("TEXT");

                    b.Property<long>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("CustomerReviewId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Deadline")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long?>("ExecutorId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("ExecutorReviewId")
                        .HasColumnType("TEXT");

                    b.Property<TimeSpan?>("ExpectedDuration")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Price")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("CustomerReviewId");

                    b.HasIndex("ExecutorId");

                    b.HasIndex("ExecutorReviewId");

                    b.ToTable("Quests");
                });

            modelBuilder.Entity("SupportUS.Web.Models.Review", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("QuestId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("SupportUS.Web.Models.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("SupportUS.Web.Models.Quest", b =>
                {
                    b.HasOne("SupportUS.Web.Models.Profile", "Customer")
                        .WithMany("CreatedQuests")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupportUS.Web.Models.Review", "CustomerReview")
                        .WithMany()
                        .HasForeignKey("CustomerReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupportUS.Web.Models.Profile", "Executor")
                        .WithMany("CompletedQuests")
                        .HasForeignKey("ExecutorId");

                    b.HasOne("SupportUS.Web.Models.Review", "ExecutorReview")
                        .WithMany()
                        .HasForeignKey("ExecutorReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("CustomerReview");

                    b.Navigation("Executor");

                    b.Navigation("ExecutorReview");
                });

            modelBuilder.Entity("SupportUS.Web.Models.Review", b =>
                {
                    b.HasOne("SupportUS.Web.Models.Profile", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("SupportUS.Web.Models.Profile", b =>
                {
                    b.Navigation("CompletedQuests");

                    b.Navigation("CreatedQuests");
                });
#pragma warning restore 612, 618
        }
    }
}
