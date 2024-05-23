// OrdersController.cs

using System.Linq;
using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShopManagementSystem.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // Action to create an order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOrder()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Handle case where the user is not logged in
                // Redirect to login page or show an error message
                return RedirectToAction("Login", "Account");
            }

            // Create a new order
            var order = new Order
            {
                CustomerId = (int)customerId,
                TotalPrice = CalculateTotalPrice(), // Implement a method to calculate total price
                OrderDate = DateTime.Now,
                Status = "pending" // Set status as pending
            };

            _context.Orders.Add(order);

            // Create OrderDetails for each book in the cart
            var cartItems = _context.Cart.Where(c => c.CustomerId == customerId).ToList();
            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Order = order,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity
                };
                _context.OrderDetails.Add(orderDetail);
            }

            _context.SaveChanges();

            // Redirect to the My Orders page
            return RedirectToAction("Index", "Orders");
        }

        private decimal CalculateTotalPrice()
        {
            // Implement method to calculate total price
            // This method should calculate the total price of items in the shopping cart
            // For simplicity, let's assume it returns a constant value for now
            return 100.0m;
        }

        // Action to display orders
        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Handle case where the user is not logged in
                // Redirect to login page or show an error message
                return RedirectToAction("Login", "Account");
            }

            var orders = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Book)
                .Where(o => o.CustomerId == customerId)
                .ToList();

            return View(orders);
        }

        //Canecl Order
        // Action to cancel an order
        public IActionResult Cancel(int id)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                // Handle case where the user is not logged in
                // Redirect to login page or show an error message
                return RedirectToAction("Login", "Account");
            }

            // Retrieve the order from the database
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .SingleOrDefault(o => o.OrderId == id && o.CustomerId == customerId);

            if (order == null)
            {
                // Order not found or does not belong to the current customer
                return NotFound();
            }

            // Check if the order status is pending
            if (order.Status != "pending")
            {
                // Order cannot be cancelled if it is not pending
                // Redirect to the My Orders page with a message indicating the inability to cancel
                TempData["Message"] = "Unable to cancel order. Order is not pending.";
                return RedirectToAction("Index");
            }

            // Update the order status to cancelled
            order.Status = "cancelled";
            _context.SaveChanges();

            // Redirect to the My Orders page with a success message
            TempData["Message"] = "Order cancelled successfully.";
            return RedirectToAction("Index");
        }
    }
}
