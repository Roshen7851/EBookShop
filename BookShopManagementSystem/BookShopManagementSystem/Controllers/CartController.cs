using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using BookShopManagementSystem.DBContext;
using System.Linq;

public class CartController : Controller
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    // AddToCart action to add items to the cart
    public IActionResult AddToCart(int id)
    {
        // Retrieve book details from database
        var book = _context.Books.Find(id);
        if (book == null)
        {
            return NotFound();
        }

        // Pass book details to the AddToCart view
        return View("AddToCart", book);
    }

    // POST: Cart/AddToCart
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddToCart(int id, int quantity)
    {
        var book = _context.Books.Find(id);
        if (book == null)
        {
            return NotFound();
        }

        // Check if the entered quantity is greater than the available quantity
        if (quantity > book.AvailableQuantity)
        {
            ModelState.AddModelError("quantity", "The entered quantity is not available.");
            return View("AddToCart", book);
        }

        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            // Handle case where the user is not logged in
            // Redirect to login page or show an error message
            return RedirectToAction("Login", "Account");
        }

        // Retrieve existing cart items from the database
        var cartItem = _context.Cart.SingleOrDefault(c => c.CustomerId == customerId && c.BookId == id);
        if (cartItem != null)
        {
            // If exists, increment quantity
            cartItem.Quantity += quantity;
        }
        else
        {
            // If not exists, add new item to the cart
            cartItem = new Cart
            {
                CustomerId = (int)customerId,
                BookId = id,
                Title = book.Title,
                Price = (decimal)book.Price,
                Quantity = quantity,
                ImagePath = book.ImagePath
            };
            _context.Cart.Add(cartItem);
        }

        // Save changes to the database
        _context.SaveChanges();

        // Redirect to the cart index page
        return RedirectToAction("Index", "Cart");
    }

    // GET: Cart/Index
    public IActionResult Index()
    {
        var customerId = HttpContext.Session.GetInt32("CustomerId");
        if (customerId == null)
        {
            // Handle case where the user is not logged in
            // Redirect to login page or show an error message
            return RedirectToAction("Login", "Account");
        }

        // Retrieve cart items from the database
        var cartItems = _context.Cart.Where(c => c.CustomerId == customerId).ToList();
        return View(cartItems);
    }

    public IActionResult RemoveFromCart(int id)
    {
        var cartItem = _context.Cart.Find(id);
        if (cartItem == null)
        {
            return NotFound();
        }

        _context.Cart.Remove(cartItem);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    // Buy Now action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult BuyNow()
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
            TotalPrice = CalculateTotalPrice(customerId.Value), // Implement a method to calculate total price
            OrderDate = DateTime.Now,
            Status = "pending" // Set status as pending
        };

        _context.Orders.Add(order);
        _context.SaveChanges();

        // Create OrderDetails for each book in the cart
        var cartItems = _context.Cart.Where(c => c.CustomerId == customerId).ToList();
        foreach (var cartItem in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = order.OrderId,
                BookId = cartItem.BookId,
                Quantity = cartItem.Quantity
            };
            _context.OrderDetails.Add(orderDetail);
        }

        // Remove items from the cart
        _context.Cart.RemoveRange(cartItems);
        _context.SaveChanges();

        // Redirect to the My Orders page
        return RedirectToAction("Index", "Orders");
    }

    private decimal CalculateTotalPrice(int customerId)
    {
        // Implement method to calculate total price based on customer's cart
        // This method should calculate the total price of items in the shopping cart
        var cartItems = _context.Cart.Where(c => c.CustomerId == customerId).ToList();
        return cartItems.Sum(item => item.Price * item.Quantity);
    }
}
