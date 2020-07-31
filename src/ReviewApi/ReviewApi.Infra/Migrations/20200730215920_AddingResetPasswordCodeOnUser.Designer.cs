﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20200730215920_AddingResetPasswordCodeOnUser")]
    partial class AddingResetPasswordCodeOnUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
#pragma warning restore 612, 618
        }
    }
}
