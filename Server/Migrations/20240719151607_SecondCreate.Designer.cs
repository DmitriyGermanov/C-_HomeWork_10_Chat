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
    [Migration("20240719151607_SecondCreate")]
    partial class SecondCreate
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

            modelBuilder.Entity("Server.Clients.Mediator", b =>
                {
                    b.Property<int>("MediatorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("varchar(13)");

                    b.HasKey("MediatorID")
                        .HasName("mediator_pkey");

                    b.ToTable("Mediator");

                    b.HasDiscriminator().HasValue("Mediator");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Server.Clients.ServerClient", b =>
                {
                    b.Property<int>("ClientID")
                        .HasColumnType("int");

                    b.Property<DateTime>("AskTime")
                        .HasColumnType("datetime(6)");

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

            modelBuilder.Entity("Server.Messenger", b =>
                {
                    b.Property<int>("MessengerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.HasKey("MessengerID")
                        .HasName("messenger_pkey");

                    b.ToTable("Messenger");
                });

            modelBuilder.Entity("Server.Clients.ClientList", b =>
                {
                    b.HasBaseType("Server.Clients.Mediator");

                    b.HasDiscriminator().HasValue("ClientList");
                });

            modelBuilder.Entity("Server.Messages.DefaultMessage", b =>
                {
                    b.HasBaseType("Server.Messages.BaseMessage");

                    b.HasDiscriminator().HasValue("DefaultMessage");
                });

            modelBuilder.Entity("Server.Clients.ServerClient", b =>
                {
                    b.HasOne("Server.Clients.Mediator", "Mediator")
                        .WithMany("Clients")
                        .HasForeignKey("ClientID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mediator");
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

            modelBuilder.Entity("Server.Clients.Mediator", b =>
                {
                    b.Navigation("Clients");
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