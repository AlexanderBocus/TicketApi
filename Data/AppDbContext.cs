using Microsoft.EntityFrameworkCore;
using TicketApi.Models;

namespace TicketApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(t => t.Description)
                .HasMaxLength(500);

            entity.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.Priority)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(t => t.CreatedAt)
                .HasConversion(
                    value => value,
                    value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
        });
    }
}