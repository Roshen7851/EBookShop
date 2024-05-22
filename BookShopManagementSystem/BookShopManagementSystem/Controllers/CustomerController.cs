using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShopManagementSystem.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult CustomerDashboard() => View();

        public IActionResult OrderBooks() => View();

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        [HttpPost]
        public async Task<IActionResult> OrderBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                BookId = book.BookId,
                UserId = 1, // Replace with actual user ID when implementing user authentication
                OrderStatus = "Pending",
                Quantity = 1, // Default quantity, you can enhance this to allow user input
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewOrders));
        }

        public async Task<IActionResult> ViewOrders()
        {
            var orders = await _context.Orders.Include(o => o.Book).Where(o => o.UserId == 1).ToListAsync(); // Replace 1 with actual user ID
            return View(orders);
        }

    }
}
