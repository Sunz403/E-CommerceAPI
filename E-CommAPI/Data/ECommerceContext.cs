using E_CommAPI.Models;
using E_CommAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace E_CommAPI.Data
{
    public class ECommerceContext : DbContext
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options)
        {
        }

       
       
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships for existing models
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            // Seed initial data
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Gaming Laptop", Description = "High-performance gaming laptop", Price = 1299.99m, StockQuantity = 15, Category = "Electronics" },
                new Product { Id = 2, Name = "Wireless Headphones", Description = "Noise-cancelling wireless headphones", Price = 199.99m, StockQuantity = 30, Category = "Electronics" },
                new Product { Id = 3, Name = "Smart Watch", Description = "Fitness and health tracking smartwatch", Price = 299.99m, StockQuantity = 25, Category = "Electronics" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "admin@store.com", Role = "Admin", ApiKey = "admin-key-12345", ApiKeyExpiry = DateTime.UtcNow.AddYears(1) },
                new User { Id = 2, Username = "customer1", Email = "customer1@email.com", Role = "Customer", ApiKey = "customer-key-67890", ApiKeyExpiry = DateTime.UtcNow.AddYears(1) }
            );
        }
    }
}
