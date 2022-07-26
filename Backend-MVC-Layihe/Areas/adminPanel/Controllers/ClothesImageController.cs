using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Areas.adminPanel.Controllers
{
    [Area("adminPanel")]
    public class ClothesImageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ClothesImageController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            List<ClothesImage> clothesImages = _context.ClothesImages.Include(c => c.Clothes).ToList();
            return View(clothesImages);
        }
        public IActionResult Create()
        {
            ViewBag.ClothesId = _context.Clothes.ToList();
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(ClothesImage clothesImage)
        {
            if (!ModelState.IsValid) return View();

            if (clothesImage.Photo is null)
            {
                ModelState.AddModelError("Photo", "Please enter image ");
                return View();
            }
            if (!clothesImage.Photo.IsImageOkay(2))
            {
                ModelState.AddModelError("Photo", "Please choose valid image file");
                return View();
            }
            clothesImage.IsMain = false;
            clothesImage.Name = await clothesImage.Photo.FileCreate(_env.WebRootPath, "assets/img");
            await _context.ClothesImages.AddAsync(clothesImage);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
    }
}
