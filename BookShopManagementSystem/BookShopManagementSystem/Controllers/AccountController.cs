using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShopManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Register for Customers
        public IActionResult RegisterCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(Customer customer)
        {
            _context.Customer.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        // Register for Admins
        public IActionResult RegisterAdmin() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(Admin admin)
        {
            _context.Admin.Add(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        // Login
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            if (role == "Admin")
            {
                var admin = await _context.Admin.FirstOrDefaultAsync(a => a.Username == username && a.Password == password);
                if (admin != null)
                {
                    // Set session variables for admin
                    HttpContext.Session.SetString("UserRole", "Admin");
                    HttpContext.Session.SetString("Username", admin.Username);
                    // Set CustomerId to 0 or any other value since it's not applicable for admins
                    HttpContext.Session.SetInt32("CustomerId", 0);
                    return RedirectToAction("AdminDashboard");
                }
            }
            else
            {
                var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Username == username && c.Password == password);
                if (customer != null)
                {
                    // Set session variables for customer
                    HttpContext.Session.SetString("UserRole", "Customer");
                    HttpContext.Session.SetString("Username", customer.Username);
                    HttpContext.Session.SetInt32("CustomerId", customer.CustomerId); // Set CustomerId to the actual customer's ID
                    return RedirectToAction("Index", "Customer");
                }
            }
            return View();
        }

        // Dashboard for Admin
        public IActionResult AdminDashboard()
        {
            return View();
        }

        // Dashboard for Customer
        public IActionResult CustomerDashboard()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View();
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            // Clear session except for necessary variables like "UserRole"
            var userRole = HttpContext.Session.GetString("UserRole");
            HttpContext.Session.Clear();
            HttpContext.Session.SetString("UserRole", userRole);

            return RedirectToAction("Login");
        }

        // Customer Management
        [HttpGet]
        public async Task<IActionResult> CustomerManagement()
        {
            var customers = await _context.Customer.ToListAsync();
            return View(customers);
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customer.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("CustomerManagement");
            }
            return View(customer);
        }


        [HttpGet]
        public async Task<IActionResult> EditCustomer(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction("CustomerManagement");
            }
            return View(customer);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost, ActionName("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomerConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("CustomerManagement");
        }

        // Book Management

    }
}