using Microsoft.EntityFrameworkCore;
using RiceDirectApi.Models;

namespace RiceDirectApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RiceListing> RiceListings => Set<RiceListing>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.PhoneNumber)
            .IsUnique();

        modelBuilder.Entity<RiceListing>()
            .HasOne(l => l.Farmer)
            .WithMany(u => u.Listings)
            .HasForeignKey(l => l.FarmerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Buyer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.Listing)
            .WithMany(l => l.Orders)
            .HasForeignKey(o => o.ListingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}