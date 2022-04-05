using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _context.Products.Take(5).OrderByDescending(p=>p.Id).ToListAsync();


            return View(products);
        }
        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();  
            ViewBag.Genres = _context.Genres.ToList();

            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            
            if (!ModelState.IsValid)
            {
                return View();
            }


            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();


            if(product.AuthorId!=null && !await _context.Authors.AnyAsync(a => a.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Dxil Etdiyiniz muellif sefdir");
                return View();
            }
            if (product.GenreId != null && !await _context.Authors.AnyAsync(a => a.Id == product.GenreId))
            {
                ModelState.AddModelError("GenreId", "Dxil Etdiyiniz janr sefdir");
                return View();
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            //return View();
            return RedirectToAction("Index");
        }
    }
}
