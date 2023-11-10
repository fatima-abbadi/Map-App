using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace TestApiJwt.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>// inherit from our customized class application user 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)//constructor for configuring the database connection and options for Entity Framework
        {
        }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between User and Shop
            modelBuilder.Entity<Shop>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shops)
                .HasForeignKey(s => s.UserId);

            // Establish the relationship between Shop and Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Shop)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.ShopId);
            // Additional configurations or constraints can be added here

            base.OnModelCreating(modelBuilder);
        }
    }
}

