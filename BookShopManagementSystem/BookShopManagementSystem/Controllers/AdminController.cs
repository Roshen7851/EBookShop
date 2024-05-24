// AdminController.cs

using System.Linq;
using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShopManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Action to manage orders
        public IActionResult OrderManagement()
        {
            var orders = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .ToList();

            return View(orders);
        }

        // Action to accept an order
        public IActionResult AcceptOrder(int orderId)
        {
            var order = _context.Orders
        .Include(o => o.OrderDetails)
        .ThenInclude(od => od.Book)
        .FirstOrDefault(o => o.OrderId == orderId);

            if (order != null)
            {
                // Update order status
                order.Status = "accepted";

                // Update book quantities
                foreach (var orderDetail in order.OrderDetails)
                {
                    var book = orderDetail.Book;
                    if (book != null)
                    {
                        // Decrease available quantity by the ordered quantity
                        book.AvailableQuantity -= orderDetail.Quantity;
                    }
                }

                // Save changes
                _context.SaveChanges();
            }

            return RedirectToAction("OrderManagement");
        }

        // Action to decline an order
        public IActionResult DeclineOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = "declined";
                _context.SaveChanges();
            }

            return RedirectToAction("OrderManagement");
        }
    }
}
