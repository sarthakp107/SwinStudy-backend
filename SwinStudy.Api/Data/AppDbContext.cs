using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Degree> Degrees => Set<Degree>();
    public DbSet<Unit> Units => Set<Unit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Degree>(e =>
        {
            e.ToTable("all_degrees");
            e.HasKey(x => x.DegreeId);
            e.Property(x => x.DegreeId).HasColumnName("degree_id");
            e.Property(x => x.DegreeName).HasColumnName("degree_name");
            e.Property(x => x.DegreeCode).HasColumnName("degree_code");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<Unit>(e =>
        {
            e.ToTable("all_units");
            e.HasKey(x => x.UnitId);
            e.Property(x => x.UnitId).HasColumnName("unit_id");
            e.Property(x => x.UnitName).HasColumnName("unit_name");
            e.Property(x => x.UnitCode).HasColumnName("unit_code");
            e.Property(x => x.CreditPoints).HasColumnName("credit_points");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });
    }
}