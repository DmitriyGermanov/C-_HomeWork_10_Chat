using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Server.Clients;
using Server.Messages;

namespace Server
{
    internal class UdpServerContext : DbContext
    {
        public DbSet<IPEndPointClient> Clients { get; set; }
        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<DefaultMessage> DefaultMessages { get; set; }
        /*        public DbSet<Mediator> Mediator { get; set; }
                public DbSet<ClientList> ClientList { get; set; }*/
        public UdpServerContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("Server=localhost;Port=3306;Database=HomeWorkBD;Uid=root;Pwd=;").UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            {
                modelBuilder.Entity<IPEndPointClient>(entity =>
                {
                    entity.HasKey(e => e.ClientID).HasName("user_pkey");
                    /*                    entity.Property(e => e.ClientID)
                                              .HasColumnName("ClientID")
                                              .ValueGeneratedOnAdd();*/
                    entity.HasIndex(e => e.ClientID).IsUnique();
                    entity.ToTable("clients");
                    entity.HasIndex(e => e.Name).IsUnique();
                    entity.Property(e => e.Name).HasColumnName("Name").HasMaxLength(255).IsRequired();
                    entity.Property(e => e.IsOnline).HasColumnType("tinyint").HasDefaultValue(false);
                    entity.Property(e => e.AskTime);
                    entity.Ignore(e => e.ClientEndPoint);
                    entity.Property(e => e.IpEndPointToString);
                    entity.Ignore(e => e.Mediator);
                });

                modelBuilder.Entity<BaseMessage>(entity =>
                {
                    entity.Property(e => e.MessageID).ValueGeneratedOnAdd();
                    entity.HasKey(e => e.MessageID).HasName("message_pkey");
                    /*                    entity.Property(e => e.MessageID)
                                              .HasColumnName("MessageID")
                                              .ValueGeneratedOnAdd();*/
                    entity.ToTable("messages");
                    entity.Property(e => e.Text).HasColumnName("Text");
                    entity.Property(e => e.DateTime).HasColumnName("DateTime");
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

                /*                modelBuilder.Entity<Mediator>(entity =>
                                {
                                    entity.ToTable("mediators");
                                    entity.HasKey(e => e.MediatorID).HasName("mediator_pkey");

                                    entity.HasDiscriminator<string>("MediatorType")
                                          .HasValue<Mediator>("Mediator")
                                         .HasValue<ClientList>("ClientList");
                        Mediator = this,
                                });
                            }*/
            }
        }
    }
}
