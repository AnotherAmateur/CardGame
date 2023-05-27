using System;
using System.Collections.Generic;
using CardGameWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CardGameWebApi.EfCore
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
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<Room> Rooms { get; set; } = null!;
        public virtual DbSet<SessionEvent> SessionEvents { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=PURPLESKY;Database=CardGameDb;Trusted_Connection=True;User id=admin;Password=1234");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameSession>(entity =>
            {
                entity.HasKey(e => e.SessionId)
                    .HasName("PK__GameSess__C9F49290A5FDC4CF");

                entity.HasOne(d => d.FirstPlayer)
                    .WithMany(p => p.GameSessionFirstPlayers)
                    .HasForeignKey(d => d.FirstPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameSessi__First__2A4B4B5E");

                entity.HasOne(d => d.SecondPlayer)
                    .WithMany(p => p.GameSessionSecondPlayers)
                    .HasForeignKey(d => d.SecondPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GameSessi__Secon__2B3F6F97");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.Property(e => e.Login).HasMaxLength(64);

                entity.Property(e => e.Password).HasMaxLength(64);
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasOne(d => d.FirstPlayer)
                    .WithMany(p => p.RoomFirstPlayers)
                    .HasForeignKey(d => d.FirstPlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Rooms__FirstPlay__2D27B809");

                entity.HasOne(d => d.SecondPlayer)
                    .WithMany(p => p.RoomSecondPlayers)
                    .HasForeignKey(d => d.SecondPlayerId)
                    .HasConstraintName("FK__Rooms__SecondPla__2E1BDC42");
            });

            modelBuilder.Entity<SessionEvent>(entity =>
            {
                entity.HasKey(e => e.GameId)
                    .HasName("PK__SessionE__2AB897FD17D76122");

                entity.Property(e => e.GameId).ValueGeneratedNever();

                entity.Property(e => e.EventDescrptn).HasMaxLength(64);

                entity.HasOne(d => d.Player)
                    .WithMany(p => p.SessionEvents)
                    .HasForeignKey(d => d.PlayerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SessionEv__Playe__2C3393D0");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
