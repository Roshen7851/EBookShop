using BookShopManagementSystem.DBContext;
using BookShopManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BookShopManagementSystem.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class BookController : Controller


    {
        private readonly AppDbContext _context;
		private readonly IWebHostEnvironment environment;

		public BookController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
			this.environment = environment;
		}

        public async Task<IActionResult> BookManagement()
        {
            var books = await _context.Books.ToListAsync();
            return View(books);
        }

        [HttpGet]
        public IActionResult CreateBook() => View();

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookDto bookDto)

        {

			if (bookDto.ImagePath == null)
			{
				ModelState.AddModelError("ImageFile", "The image file id required");
			}

			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			newFileName += Path.GetExtension(bookDto.ImagePath!.FileName);

			string imageFullPath = environment.WebRootPath + "/bookImages/" + newFileName;
			using (var stream = System.IO.File.OpenWrite(imageFullPath))
			{
				bookDto.ImagePath.CopyTo(stream);
			}


			if (ModelState.IsValid)
            {

                Book addBook = new Book()
                {
					Title = bookDto.Title,
					Author = bookDto.Author,
                    Description = bookDto.Description,
                    AvailableQuantity = bookDto.AvailableQuantity,
                    BookCategory = bookDto.BookCategory,
                    ISBN = bookDto.ISBN,
                    Price = bookDto.Price,
                    ImagePath = newFileName,


                };


                _context.Books.Add(addBook);
                await _context.SaveChangesAsync();
                return RedirectToAction("BookManagement");


            }
            return View(bookDto);
        }



        public IActionResult BookEdit(int id)
        {
            var book = _context.Books.Find(id);

            if (book == null)
            {
                return RedirectToAction("BookManagement");
            }

            var bookDto = new BookDto()
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                AvailableQuantity = book.AvailableQuantity,
                BookCategory = book.BookCategory,
                ISBN = book.ISBN,
                Price = book.Price,
                PublishDate = book.PublishDate,
                
            };

            ViewData["BookId"] = book.BookId;
            ViewData["ImageFileName"] = book.ImagePath;
            ViewData["PublishDate"] = book.PublishDate;

            return View(bookDto);
        }

        [HttpPost]
        public IActionResult BookEdit(int id, BookDto bookDto)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return RedirectToAction("BookManagement");
            }

            if (!ModelState.IsValid)
            {

                ViewData["BookId"] = book.BookId;
                ViewData["ImageFileName"] = book.ImagePath;
                ViewData["PublishDate"] = book.PublishDate;

                return View(bookDto);
            }

            //update the image file if we have a new image file
            string newFileName = book.ImagePath;
            if (bookDto.ImagePath != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(bookDto.ImagePath.FileName);

                string imageFullPath = environment.WebRootPath + "/bookImages/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    bookDto.ImagePath.CopyTo(stream);
                }

                //delete the old image
                String oldImageFullPath = environment.WebRootPath + "/bookImages/" + book.ImagePath;
                System.IO.File.Delete(oldImageFullPath);

            }


            book.Title = bookDto.Title;
	        book.Author = bookDto.Author;
            book.Description = bookDto.Description;
            book.AvailableQuantity = bookDto.AvailableQuantity;
            book.BookCategory = bookDto.BookCategory;
            book.ISBN = bookDto.ISBN;
            book.Price = bookDto.Price;
            book.PublishDate = bookDto.PublishDate;
            book.ImagePath = newFileName;

            _context.SaveChanges();

            return RedirectToAction("BookManagement");


        }

        public IActionResult DeleteBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return RedirectToAction("BookManagement");

            }

            //delete image from folder
            String imageFullPath = environment.WebRootPath + "/bookImages/" + book.ImagePath;
            System.IO.File.Delete(imageFullPath);

            _context.Books.Remove(book);
            _context.SaveChanges();

            return RedirectToAction("BookManagement");

        }
    }
}
