using Microsoft.EntityFrameworkCore;
using Weblog.Domain;

namespace Weblog.Application;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Article> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(e =>
        {
            e.HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .IsRequired();

            e.HasMany(a => a.Authors)
                .WithOne()
                .HasForeignKey(a => a.ArticleId);
        });

        modelBuilder.Entity<ArticleAuthor>(e =>
        {
            e.Property(a => a.UserId).IsRequired();
            e.HasKey(a => new { a.ArticleId, a.UserId });
        });

        base.OnModelCreating(modelBuilder);
    }
}
