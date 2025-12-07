using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SwedesRankTracker.Models;

public partial class ClanDbContext : DbContext
{
    public ClanDbContext()
    {
    }

    public ClanDbContext(DbContextOptions<ClanDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Rank> Ranks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=Swedes;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Member__0CF04B18C58B4FB5");

            entity.ToTable("Member");

            entity.Property(e => e.LastUpdated).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.ToTable("Rank");

            entity.Property(e => e.MinPets).HasColumnName("Min_Pets");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
