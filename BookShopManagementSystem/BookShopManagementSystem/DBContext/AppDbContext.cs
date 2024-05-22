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
        public DbSet<Book> Books { get; set; }
    }
}
