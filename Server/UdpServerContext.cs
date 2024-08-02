using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Server.Clients;
using Server.Messages;
using System.Net;

namespace Server
{
    internal class UdpServerContext : DbContext
    {
        public DbSet<ClientBase> Clients { get; set; }
        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<DefaultMessage> DefaultMessages { get; set; }

        public UdpServerContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=HomeWorkBD;Uid=root;Pwd=;")
                          .UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientBase>(entity =>
            {
                entity.HasKey(e => e.ClientID).HasName("PK_ClientID");
                entity.HasIndex(e => e.ClientID).IsUnique();
                entity.ToTable("Clients");
                entity.Property(e => e.Name)
                      .HasColumnName("Name")
                      .HasMaxLength(255)
                      .IsRequired();
                entity.HasDiscriminator<string>("ClientType")
                      .HasValue<IPEndPointClient<IPEndPoint>>("IPEndPointClient").HasValue < NetMqClient<byte[]>>("NetMqClient"); ;
                entity.Ignore(e => e.Mediator);
            });

            modelBuilder.Entity<IPEndPointClient<IPEndPoint>>(entity =>
            {
                entity.Property(e => e.IpEndPointToString)
                      .HasColumnName("IpEndPointToString");
                entity.Ignore(e => e.ClientEndPoint);
            });
            modelBuilder.Entity<NetMqClient<byte[]>>(entity =>
            {
                entity.Property(e => e.ClientNetId)
                         .HasColumnName("ClientNetID");
            });


            modelBuilder.Entity<BaseMessage>(entity =>
            {
                entity.HasKey(e => e.MessageID).HasName("PK_MessageID");
                entity.ToTable("Messages");
                entity.Property(e => e.Text)
                      .HasColumnName("Text")
                      .IsRequired();
                entity.Property(e => e.DateTime)
                      .HasColumnName("DateTime")
                      .IsRequired();
                entity.HasOne(e => e.ClientTo)
                      .WithMany(e => e.MessagesTo)
                      .HasForeignKey(e => e.UserIdTo);
                entity.HasOne(e => e.ClientFrom)
                      .WithMany(e => e.MessagesFrom)
                      .HasForeignKey(e => e.UserIDFrom);
                entity.Ignore(e => e.Ask);
                entity.Ignore(e => e.DisconnectRequest);
                entity.Ignore(e => e.LocalEndPoint);
                entity.Ignore(e => e.LocalEndPointString);
                entity.Ignore(e => e.UserIsOnline);
                entity.Ignore(e => e.UserDoesNotExist);
                entity.Ignore(e => e.NicknameTo);
                entity.Ignore(e => e.NicknameFrom);
            });
        }
    }
}