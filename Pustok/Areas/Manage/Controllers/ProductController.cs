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

        private bool CheckId(object id)
        {
            if (id == null)
            {
                return false;
            }
            return true;
        }

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _context.Products.Take(5).Where(p=>!p.IsDeleted).OrderByDescending(p=>p.Id).ToListAsync();


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

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(a=>a.Author)
                .Include(g=>g.Genre)
                .FirstOrDefaultAsync(p=>p.Id == id && !p.IsDeleted);
            if (product == null) return NotFound();

            return View(product);
        }

        public async Task<IActionResult> Update(int? id)
        {
            if (!CheckId(id)) return BadRequest();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();

            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if(!CheckId(product)) return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? id,Product product)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (id == null || id != product.Id) return BadRequest();
            Product dbProduct = await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);
            if (dbProduct == null) return NotFound();
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();


            if (product.AuthorId != null && !await _context.Authors.AnyAsync(a => a.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Dxil Etdiyiniz muellif sefdir");
                return View();
            }
            if (product.GenreId != null && !await _context.Authors.AnyAsync(a => a.Id == product.GenreId))
            {
                ModelState.AddModelError("GenreId", "Dxil Etdiyiniz janr sefdir");
                return View();
            }

            dbProduct.Title = product.Title;
            dbProduct.Price = product.Price;
            dbProduct.MainImage = product.MainImage;
            dbProduct.ProductImages = product.ProductImages;
            dbProduct.DiscountPrice = product.DiscountPrice;
            dbProduct.GenreId = product.GenreId;
            dbProduct.AuthorId = product.AuthorId;
            dbProduct.IsArrival = product.IsArrival;
            dbProduct.IsFeature = product.IsFeature;
            dbProduct.IsMostView = product.IsMostView;
            dbProduct.HoverImage = product.HoverImage;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            Product product = await _context.Products
                .Include(a => a.Author)
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (!CheckId(product)) return NotFound();

            return View(product);
        }
        public async Task<IActionResult> DeleteProduct(int? id)
        {

            if (id == null) return BadRequest();

            Product product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (!CheckId(product)) return NotFound();

            //_context.Products.Remove(product);

            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
