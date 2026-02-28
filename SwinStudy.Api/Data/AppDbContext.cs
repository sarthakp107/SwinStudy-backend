using Microsoft.EntityFrameworkCore;
using SwinStudy.Api.Models;

namespace SwinStudy.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Degree> Degrees => Set<Degree>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UserSavedFlashcard> UserSavedFlashcards => Set<UserSavedFlashcard>();
    public DbSet<AllFlashcard> AllFlashcards => Set<AllFlashcard>();
    public DbSet<UserGeneratedFlashcard> UserGeneratedFlashcards => Set<UserGeneratedFlashcard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Email).HasColumnName("email");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(x => x.Email).IsUnique();
        });

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

        modelBuilder.Entity<UserSavedFlashcard>(e =>
        {
            e.ToTable("m_usersavedflashcards");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("m_usersavedflashcards_pkey");
            e.Property(x => x.UserId).HasColumnName("userid");
            e.Property(x => x.Question).HasColumnName("question");
            e.Property(x => x.Answer).HasColumnName("answer");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<AllFlashcard>(e =>
        {
            e.ToTable("m_allflashcards");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("m_allflashcards_pkey");
            e.Property(x => x.Question).HasColumnName("question");
            e.Property(x => x.Answer).HasColumnName("answer");
            e.Property(x => x.CreatedDate).HasColumnName("created_date");
        });

        modelBuilder.Entity<UserGeneratedFlashcard>(e =>
        {
            e.ToTable("m_usergeneratedflashcards");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasColumnName("m_usergeneratedflashcards_pkey");
            e.Property(x => x.UserId).HasColumnName("userid");
            e.Property(x => x.QnaReferenceId).HasColumnName("qnareference");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");

            e.HasOne(x => x.Flashcard)
             .WithMany(f => f.GeneratedByUsers)
             .HasForeignKey(x => x.QnaReferenceId)
             .HasConstraintName("fk_m_usergeneratedflashcards_qnaref");
        });
    }
}