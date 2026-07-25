using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Outty.Api.Data;

public partial class OuttyDbContext : DbContext
{
    public OuttyDbContext(DbContextOptions<OuttyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExperienceLevel> ExperienceLevels { get; set; }

    public virtual DbSet<Goal> Goals { get; set; }

    public virtual DbSet<Interest> Interests { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<ProfileInterest> ProfileInterests { get; set; }

    public virtual DbSet<ProfilePhoto> ProfilePhotos { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExperienceLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Experien__3214EC07E67C48FF");

            entity.ToTable("ExperienceLevel");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.ExperienceLevel1)
                .HasMaxLength(30)
                .HasColumnName("ExperienceLevel");
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Goals__3214EC0716AD728E");

            entity.HasIndex(e => e.Name, "UQ__Goals__737584F6C732F116").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Interest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Interest__3214EC0788383DD8");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Profiles__3214EC079C6C5579");

            entity.HasIndex(e => e.UserId, "UQ__Profiles__1788CC4DB6141892").IsUnique();

            entity.Property(e => e.Bio).HasMaxLength(300);
            entity.Property(e => e.City).HasMaxLength(150);
            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.PreferredDistance).HasMaxLength(30);
            entity.Property(e => e.Pronouns).HasMaxLength(30);
            entity.Property(e => e.SearchRadiusMiles).HasDefaultValue(25);
            entity.Property(e => e.State)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.UpdatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.StateNavigation).WithMany(p => p.Profiles)
                .HasPrincipalKey(p => p.Abbreviation)
                .HasForeignKey(d => d.State)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Profiles__State__7A672E12");

            entity.HasOne(d => d.User).WithOne(p => p.Profile)
                .HasForeignKey<Profile>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Profiles__UserId__797309D9");

            entity.HasMany(d => d.Goals).WithMany(p => p.Profiles)
                .UsingEntity<Dictionary<string, object>>(
                    "ProfileGoal",
                    r => r.HasOne<Goal>().WithMany()
                        .HasForeignKey("GoalId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProfileGo__GoalI__0F624AF8"),
                    l => l.HasOne<Profile>().WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__ProfileGo__Profi__0E6E26BF"),
                    j =>
                    {
                        j.HasKey("ProfileId", "GoalId").HasName("PK__ProfileG__21A8771919449138");
                        j.ToTable("ProfileGoals");
                    });
        });

        modelBuilder.Entity<ProfileInterest>(entity =>
        {
            entity.HasKey(e => new { e.ProfileId, e.InterestId });

            entity.HasOne(d => d.ExperienceLevel).WithMany(p => p.ProfileInterests)
                .HasForeignKey(d => d.ExperienceLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProfileIn__Exper__08B54D69");

            entity.HasOne(d => d.Interest).WithMany(p => p.ProfileInterests)
                .HasForeignKey(d => d.InterestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProfileIn__Inter__07C12930");

            entity.HasOne(d => d.Profile).WithMany(p => p.ProfileInterests)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProfileIn__Profi__06CD04F7");
        });

        modelBuilder.Entity<ProfilePhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProfileP__3214EC0767F6CF95");

            entity.Property(e => e.BlobUrl).HasMaxLength(500);
            entity.Property(e => e.FileName).HasMaxLength(260);

            entity.HasOne(d => d.Profile).WithMany(p => p.ProfilePhotos)
                .HasForeignKey(d => d.ProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProfilePh__Profi__7F2BE32F");
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__States__3214EC07AE9BB063");

            entity.HasIndex(e => e.Name, "UQ__States__737584F65996DB36").IsUnique();

            entity.HasIndex(e => e.Abbreviation, "UQ__States__9C41170EE396952E").IsUnique();

            entity.Property(e => e.Abbreviation)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC071A36E780");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105347ABCBCCC").IsUnique();

            entity.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
