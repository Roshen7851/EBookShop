using BookShopManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BookShopManagementSystem.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base (options)
        {
            
        }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Customer> Customer { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Cart> Cart { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasKey(c => c.CartId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithMany(cu => cu.Carts)
                .HasForeignKey(c => c.CustomerId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
