﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Migrations
{
    [DbContext(typeof(MainContext))]
    partial class MainContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ReviewApi.Domain.Entities.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnName("filename")
                        .HasColumnType("nvarchar(41)")
                        .HasMaxLength(41);

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnName("filepath")
                        .HasColumnType("nvarchar(300)")
                        .HasMaxLength(300);

                    b.Property<Guid>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(36)
                        .HasDefaultValue(null);

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("images");
                });

            modelBuilder.Entity("ReviewApi.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConfirmationCode")
                        .HasColumnName("confirmation_code")
                        .HasColumnType("nvarchar(8)")
                        .HasMaxLength(8);

                    b.Property<bool>("Confirmed")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("confirmed")
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("email")
                        .HasColumnType("nvarchar(255)")
                        .HasMaxLength(255);

                    b.Property<Guid>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("image_id")
                        .HasColumnType("uniqueidentifier")
                        .HasMaxLength(36)
                        .HasDefaultValue(null);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasColumnType("nvarchar(150)")
                        .HasMaxLength(150);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnName("password_hash")
                        .HasColumnType("nvarchar(64)")
                        .HasMaxLength(64);

                    b.Property<string>("ResetPasswordCode")
                        .HasColumnName("reset_password_code")
                        .HasColumnType("nvarchar(8)")
                        .HasMaxLength(8);

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("ReviewApi.Domain.Entities.Image", b =>
                {
                    b.HasOne("ReviewApi.Domain.Entities.User", "User")
                        .WithOne("Image")
                        .HasForeignKey("ReviewApi.Domain.Entities.Image", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
