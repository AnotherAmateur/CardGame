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
                // nothing here
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__GameSess__C9F492909A6BB32C");

                entity.HasOne(d => d.FirstPlayer)
                    .WithMany(p => p.GameSessionFirstPlayers)
                    .HasForeignKey(d => d.FirstPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameSessi__First__286302EC");

                entity.HasOne(d => d.SecondPlayer)
                    .WithMany(p => p.GameSessionSecondPlayers)
                    .HasForeignKey(d => d.SecondPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameSessi__Secon__29572725");

                entity.HasOne(d => d.Winner)
                    .WithMany(p => p.GameSessionWinners)
                    .HasForeignKey(d => d.WinnerId)
                    .HasConstraintName("FK__GameSessi__Winne__2A4B4B5E");
            });

            modelBuilder.Entity<Lobby>(entity =>
            {
                entity.HasKey(e => e.Master)
                    .HasName("PK__Lobbies__91FE357073DFB127");

                entity.Property(e => e.Master).ValueGeneratedNever();

                entity.HasOne(d => d.MasterNavigation)
                    .WithOne(p => p.Lobby)
                    .HasForeignKey<Lobby>(d => d.Master)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Lobbies__Master__2B3F6F97");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Login).HasMaxLength(64);

                entity.Property(e => e.Password).HasMaxLength(64);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
