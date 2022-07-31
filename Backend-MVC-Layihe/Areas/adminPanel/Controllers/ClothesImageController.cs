using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.Utilities;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Moderator,Admin")]

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

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.ClothesId = _context.Clothes.Include(c=>c.ClothesImages).ToList();
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ClothesImage clothesImage)
        {
            ViewBag.ClothesId = _context.Clothes.Include(c => c.ClothesImages).ToList();

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (clothesImage.Photo is null)
            {
                ModelState.AddModelError("Photo", "Please enter image");
                return View();
            }
            if (!clothesImage.Photo.IsImageOkay(2))
            {
                ModelState.AddModelError("Photo", "Please choose valid image file");
                return View();
            }
            
            clothesImage.Name = await clothesImage.Photo.FileCreate(_env.WebRootPath, "assets/img");
            await _context.ClothesImages.AddAsync(clothesImage);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {

            if (id is null || id == 0) return NotFound();

            ClothesImage existed = _context.ClothesImages.FirstOrDefault(c => c.Id == id);
            if (existed is null) return NotFound();

            ViewBag.ClothesId = _context.Clothes.Include(c => c.ClothesImages).ToList();
            return View(existed);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int? id,ClothesImage newClothesImage)
        {
            ClothesImage existed = await _context.ClothesImages.FirstOrDefaultAsync(c => c.Id == id);
            if (existed is null) return NotFound();
            ViewBag.ClothesId = _context.Clothes.Include(c => c.ClothesImages).ToList();
            if (!ModelState.IsValid) return View(existed);

            if(newClothesImage.Photo is null)
            {
                string filename = existed.Name;
                _context.Entry(existed).CurrentValues.SetValues(newClothesImage);
                existed.Name = filename;
            }
            else
            {
                if (!newClothesImage.Photo.IsImageOkay(2))
                {
                    ModelState.AddModelError("Photo", "Please choose valid image file");
                    return View(existed);
                }

                FileValidator.FileDelete(_env.WebRootPath, "assets/img", existed.Name);
                _context.Entry(existed).CurrentValues.SetValues(newClothesImage);
                existed.Name = await newClothesImage.Photo.FileCreate(_env.WebRootPath, "assets/img");
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();
            ClothesImage existed = await _context.ClothesImages.FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            _context.ClothesImages.Remove(existed);
            FileValidator.FileDelete(_env.WebRootPath, "assets/img", existed.Name);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ClothesImage existed = await _context.ClothesImages.Include(c=>c.Clothes).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }
    }
}
