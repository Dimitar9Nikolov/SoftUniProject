using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftUniProject.Data.Models;

namespace SoftUniProject.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Price> Prices { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(u => u.PlacedOrders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasOne(o => o.DeliveryMan)
            .WithMany(u => u.AcceptedDeliveries)
            .HasForeignKey(o => o.DeliveryManId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Order>()
            .HasOne(o => o.Price)
            .WithOne(p => p.Order)
            .HasForeignKey<Price>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}