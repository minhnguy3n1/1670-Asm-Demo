﻿#nullable disable
using Demo6.Areas.Identity.Data;
using Demo6.Data;
using Demo6.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Demo6.Controllers
{
    public class BooksController : Controller
    {
        private readonly UserContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BooksController(UserContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            AppUser thisUser = await _userManager.GetUserAsync(HttpContext.User);
            Store thisStore = await _context.Stores.FirstOrDefaultAsync(s => s.UId == thisUser.Id);
            var books = _context.Books
                .Where(b => b.StoreId == thisStore.Id)
                .Include(b => b.Store);               
            return View(await books.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Seller")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Isbn,Title,Pages,Author,CategoryId,Price,Decs")] Book book, IFormFile image)
        {
            if (image != null)
            {

                //Set Key Name
                string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                //Get url To Save
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", ImageName);

                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            AppUser thisUser = await _userManager.GetUserAsync(HttpContext.User);
            Store thisStore = await _context.Stores.FirstOrDefaultAsync(s => s.UId == thisUser.Id); 
            book.StoreId = thisStore.Id;
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", book.StoreId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Isbn,Title,Pages,Author,Category,Price,Decs,Imgurl,StoreId")] Book book, IFormFile image)
        {
            if (image != null)
            {

                //Set Key Name
                string ImageName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                //Get url To Save
                string SavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", ImageName);

                using (var stream = new FileStream(SavePath, FileMode.Create))
                {
                    image.CopyTo(stream);
                }
                book.Imgurl = "img/" + ImageName;
            }
            if (id != book.Isbn)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Isbn))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.Stores, "Id", "Id", book.StoreId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Store)
                .FirstOrDefaultAsync(m => m.Isbn == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(string id)
        {
            return _context.Books.Any(e => e.Isbn == id);
        }
    }
}
