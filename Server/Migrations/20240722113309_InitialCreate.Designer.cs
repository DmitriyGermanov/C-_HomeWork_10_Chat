﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server;

#nullable disable

namespace Server.Migrations
{
    [DbContext(typeof(UdpServerContext))]
    [Migration("20240722113309_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Server.Clients.ServerClient", b =>
                {
                    b.Property<int>("ClientID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("AskTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("IpEndPointToString")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<sbyte>("IsOnline")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasDefaultValue((sbyte)0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)")
                        .HasColumnName("Name");

                    b.HasKey("ClientID")
                        .HasName("user_pkey");

                    b.HasIndex("ClientID")
                        .IsUnique();

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("clients", (string)null);
                });

            modelBuilder.Entity("Server.Messages.BaseMessage", b =>
                {
                    b.Property<int>("MessageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("DateTime");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("varchar(21)");

                    b.Property<string>("Text")
                        .HasColumnType("longtext")
                        .HasColumnName("Text");

                    b.Property<int?>("UserIDFrom")
                        .HasColumnType("int");

                    b.Property<int?>("UserIdTo")
                        .HasColumnType("int");

                    b.HasKey("MessageID")
                        .HasName("message_pkey");

                    b.HasIndex("UserIDFrom");

                    b.HasIndex("UserIdTo");

                    b.ToTable("messages", (string)null);

                    b.HasDiscriminator().HasValue("BaseMessage");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Server.Messages.DefaultMessage", b =>
                {
                    b.HasBaseType("Server.Messages.BaseMessage");

                    b.HasDiscriminator().HasValue("DefaultMessage");
                });

            modelBuilder.Entity("Server.Messages.BaseMessage", b =>
                {
                    b.HasOne("Server.Clients.ServerClient", "ClientFrom")
                        .WithMany("MessagesFrom")
                        .HasForeignKey("UserIDFrom");

                    b.HasOne("Server.Clients.ServerClient", "ClientTo")
                        .WithMany("MessagesTo")
                        .HasForeignKey("UserIdTo");

                    b.Navigation("ClientFrom");

                    b.Navigation("ClientTo");
                });

            modelBuilder.Entity("Server.Clients.ServerClient", b =>
                {
                    b.Navigation("MessagesFrom");

                    b.Navigation("MessagesTo");
                });
#pragma warning restore 612, 618
        }
    }
}