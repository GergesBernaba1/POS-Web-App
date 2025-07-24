using Microsoft.EntityFrameworkCore;
using POS.API.Models;

namespace POS.API.Data
{
    public class POSDbContext : DbContext
    {
        public POSDbContext(DbContextOptions<POSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Sale>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sales)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(si => si.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            modelBuilder.Entity<Sale>()
                .HasIndex(s => s.SaleNumber)
                .IsUnique();

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic items and gadgets" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items" },
                new Category { Id = 3, Name = "Food & Beverages", Description = "Food and drink items" },
                new Category { Id = 4, Name = "Books", Description = "Books and publications" }
            );

            // Seed default admin user
            modelBuilder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    Username = "admin", 
                    Email = "admin@posapp.com",
                    FirstName = "Admin",
                    LastName = "User",
                    Role = POS.API.Enums.UserRole.Admin,
                    IsActive = true,
                    // Password: "admin123" - This will be hashed in the service
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    CreatedAt = DateTime.UtcNow
                }
            );

            // Seed some sample products
            modelBuilder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, 
                    Name = "Laptop", 
                    Description = "High-performance laptop", 
                    SKU = "ELEC001", 
                    Price = 999.99m, 
                    Cost = 750.00m, 
                    StockQuantity = 10, 
                    MinStockLevel = 2, 
                    CategoryId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "T-Shirt", 
                    Description = "Cotton T-Shirt", 
                    SKU = "CLOTH001", 
                    Price = 19.99m, 
                    Cost = 12.00m, 
                    StockQuantity = 50, 
                    MinStockLevel = 10, 
                    CategoryId = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new Product 
                { 
                    Id = 3, 
                    Name = "Coffee", 
                    Description = "Premium coffee beans", 
                    SKU = "FOOD001", 
                    Price = 12.99m, 
                    Cost = 8.00m, 
                    StockQuantity = 25, 
                    MinStockLevel = 5, 
                    CategoryId = 3,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
