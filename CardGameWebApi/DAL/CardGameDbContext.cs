using System;
using System.Collections.Generic;
using CardGameWebApi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CardGameWebApi.DAL
{
    public partial class CardGameDbContext : DbContext
    {
        public CardGameDbContext()
        {
        }

        public CardGameDbContext(DbContextOptions<CardGameDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<GameSession> GameSessions { get; set; } = null!;
        public virtual DbSet<Lobby> Lobbies { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=SQL6030.site4now.net;Initial Catalog=db_a99f40_cardgamedb;User Id=db_a99f40_cardgamedb_admin;Password=lomkost321");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__GameSess__C9F49290A7F56B8F");

                entity.HasOne(d => d.FirstPlayer)
                    .WithMany(p => p.GameSessionFirstPlayers)
                    .HasForeignKey(d => d.FirstPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameSessi__First__29572725");

                entity.HasOne(d => d.SecondPlayer)
                    .WithMany(p => p.GameSessionSecondPlayers)
                    .HasForeignKey(d => d.SecondPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameSessi__Secon__2A4B4B5E");

                entity.HasOne(d => d.Winner)
                    .WithMany(p => p.GameSessionWinners)
                    .HasForeignKey(d => d.WinnerId)
                    .HasConstraintName("FK__GameSessi__Winne__2B3F6F97");
            });

            modelBuilder.Entity<Lobby>(entity =>
            {
                entity.HasKey(e => e.Master)
                    .HasName("PK__Lobbies__91FE3570A3AE218B");

                entity.Property(e => e.Master).ValueGeneratedNever();

                entity.HasOne(d => d.MasterNavigation)
                    .WithOne(p => p.Lobby)
                    .HasForeignKey<Lobby>(d => d.Master)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lobbies__Master__2C3393D0");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Login, "UQ__Users__5E55825BE4EA783A")
                    .IsUnique();

                entity.Property(e => e.Login).HasMaxLength(64);

                entity.Property(e => e.Password).HasMaxLength(64);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
