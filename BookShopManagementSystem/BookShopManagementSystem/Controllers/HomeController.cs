using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BookShopManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        //public async Task<IActionResult> Index()
        //{
            //Search
            // Action to display the home page with all books or filtered books based on search query
            public IActionResult Index(string query)
            {
                IQueryable<Book> books = _context.Books;

                if (!string.IsNullOrEmpty(query))
                {
                    books = books.Where(b => b.Title.Contains(query) ||
                                             b.BookCategory.Contains(query) ||
                                             b.Author.Contains(query));
                }

                return View(books.ToList());
            }
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
