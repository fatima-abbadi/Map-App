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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the relationship between User and Shop
            modelBuilder.Entity<Shop>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shops)
                .HasForeignKey(s => s.UserId);

            // Additional configurations or constraints can be added here
            modelBuilder.Entity<Product>()
             .HasOne(p => p.Category)
             .WithMany(c => c.Products)
             .HasForeignKey(p => p.CategoryId);

            // Establish the relationship between Shop and Category
            modelBuilder.Entity<Shop>()
                .HasMany(s => s.Categories)
                .WithOne(c => c.Shop)
                .HasForeignKey(c => c.ShopId);

            modelBuilder.Entity<Cart>()
               .HasOne(c => c.User)
               .WithMany(u => u.Carts)
               .HasForeignKey(c => c.UserId);


            modelBuilder.Entity<Cart>()
                .Property(c => c.TotalPrice)
                .HasColumnType("decimal(18,2)");


            // Configure the many-to-many relationship between User and Shop for favorites
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.ShopId });

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Cascade delete when User is deleted

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Shop)
                .WithMany(s => s.Favorites)
                .HasForeignKey(f => f.ShopId)
                .OnDelete(DeleteBehavior.ClientSetNull); // Cascade delete when Shop is deleted


        }
    }
}

