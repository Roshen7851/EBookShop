using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BookShopManagementSystem.Controllers
{
    public class CartController : Controller
    {

        private readonly AppDbContext _context;

        private readonly IHttpContextAccessor _contextAccessor;

        public CartController(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index(AppDbContext context)
        {
            return View();
        }

        public IActionResult AddtoCart(int bookId)
        {

            List<Cart> cartList = new List<Cart>();
            string cartString = _contextAccessor.HttpContext.Session.GetString("carts");
            cartList = JsonConvert.DeserializeObject<List<Cart>>(cartString);

            if (cartList == null)
            {
                List<Cart> carts = new List<Cart>();
                Cart cart = new Cart();
                cart.book = _context.Books.Find(bookId);
                cart.Quantity = 1;
                carts.Add(cart);

                string cartAddString = JsonConvert.SerializeObject(carts);
                _contextAccessor.HttpContext.Session.SetString("carts", cartAddString);

            }
            else
            {
                //List<Cart> cart
            }

            return RedirectToAction("Customer", "Index");
        }


    }
}
