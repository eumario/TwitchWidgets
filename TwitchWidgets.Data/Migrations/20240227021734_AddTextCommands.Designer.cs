﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TwitchWidgets.Data.Context;

#nullable disable

namespace TwitchWidgets.Data.Migrations
{
    [DbContext(typeof(TwitchWidgetsContext))]
    [Migration("20240227021734_AddTextCommands")]
    partial class AddTextCommands
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("TwitchWidgets.Data.Models.Secret", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BotAuthToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotProfilePic")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotRefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("BotUserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientSecret")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamerAuthToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamerDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamerProfilePic")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamerRefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamerUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("StreamerUserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Secrets");
                });

            modelBuilder.Entity("TwitchWidgets.Data.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("BValue")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("DValue")
                        .HasColumnType("REAL");

                    b.Property<float?>("FValue")
                        .HasColumnType("REAL");

                    b.Property<int?>("IValue")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SValue")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("TwitchWidgets.Data.Models.TextCommand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CommandAlias")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CommandDescription")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CommandHelp")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CommandName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Commands");
                });
#pragma warning restore 612, 618
        }
    }
}