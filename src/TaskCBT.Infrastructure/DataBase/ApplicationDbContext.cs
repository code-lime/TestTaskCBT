using Microsoft.EntityFrameworkCore;
using TaskCBT.Application.Common.Interfaces;
using TaskCBT.Domain.Entities;
using Innofactor.EfCoreJsonValueConverter;

namespace TaskCBT.Infrastructure.DataBase;

public partial class ApplicationDbContext : DbContext, IContext
{
    public ApplicationDbContext()
        => Database.EnsureCreated();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        => Database.EnsureCreated();

    public DbSet<Auth> Auths { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventSubscriber> EventSubscribers { get; set; }
    public DbSet<User> Users { get; set; }

    public DbContext DbContext => this;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Auth>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Identity).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(15);
                entity.Property(e => e.Identity).HasMaxLength(254);
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(15);
                entity.Property(e => e.Data);
                entity.Property(e => e.Salt);

                entity
                    .HasOne(e => e.User)
                    .WithOne(e => e.Auth)
                    .HasForeignKey<User>(e => e.AuthId);
                entity
                    .HasMany(e => e.RefreshTokens)
                    .WithOne(e => e.Auth)
                    .HasForeignKey(e => e.AuthId);
            });
        modelBuilder
            .Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Token).IsUnique();

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Token).HasMaxLength(100);

                entity
                    .HasOne(e => e.Auth)
                    .WithMany(e => e.RefreshTokens)
                    .HasForeignKey(e => e.AuthId);
            });
        modelBuilder
            .Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title).HasMaxLength(100);
                entity.Property(e => e.Type).HasMaxLength(100);
                entity.Property(e => e.Time);
                entity.Property(e => e.SubscribersLimit);
                entity.Property(e => e.Fields)
                    .HasJsonValueConversion();

                entity
                    .HasOne(e => e.Owner)
                    .WithMany(e => e.OwnerEvents)
                    .HasForeignKey(e => e.OwnerId);
                entity
                    .HasMany(e => e.Subscribers)
                    .WithOne(e => e.Event)
                    .HasForeignKey(e => e.EventId);
            });
        modelBuilder
            .Entity<EventSubscriber>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity
                    .HasOne(e => e.User)
                    .WithMany(e => e.Subscriptions)
                    .HasForeignKey(e => e.UserId);
                entity
                    .HasOne(e => e.Event)
                    .WithMany(e => e.Subscribers)
                    .HasForeignKey(e => e.EventId);
            });
        modelBuilder
            .Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Fields)
                    .HasJsonValueConversion();

                entity
                    .HasOne(e => e.Auth)
                    .WithOne(e => e.User)
                    .HasForeignKey<User>(e => e.AuthId);
                entity
                    .HasMany(e => e.OwnerEvents)
                    .WithOne(e => e.Owner)
                    .HasForeignKey(e => e.OwnerId);
                entity
                    .HasMany(e => e.Subscriptions)
                    .WithOne(e => e.User)
                    .HasForeignKey(e => e.UserId);
            });

        OnModelCreatingPartial(modelBuilder);
    }
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
