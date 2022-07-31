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

    public class ColorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ColorController(ApplicationDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            byte ItemsPerPage = 6;
            ViewBag.CurrPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_context.Colors.Count() / ItemsPerPage);

            List<Color> colors = _context.Colors.OrderByDescending(c => c.Id)
                .Skip((page-1)*ItemsPerPage).Take(ItemsPerPage).ToList();
            return View(colors);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid) return View();
            if(color.Photo is null)
            {
                ModelState.AddModelError("Photo", "You have to upload an image");
                return View();
            }
            Color existed = await _context.Colors.FirstOrDefaultAsync(x =>
            x.Name.ToLower().Trim() == color.Name.ToLower().Trim());

            if (existed != null)
            {
                ModelState.AddModelError("Name", "This color already exists");
                return View();
            }
            if (!color.Photo.IsImageOkay(2))
            {
                ModelState.AddModelError("Photo", "File size invalid. Please choose another image");
                return View();
            }
            color.Image =  await color.Photo.FileCreate(_env.WebRootPath, "assets/img/colors");
            _context.Colors.Add(color);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Color existed = _context.Colors.FirstOrDefault(x => x.Id == id);
            if (existed is null) return NotFound();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int? id, Color newColor)
        {
            if (id is null || id == 0) return NotFound();
            if (!ModelState.IsValid) return View();

            Color existed = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (existed is null) return BadRequest();

            if (newColor.Photo is null)
            {
                string filename = existed.Image;
                _context.Entry(existed).CurrentValues.SetValues(newColor);
                existed.Image = filename;
            }
            else
            {
                if (!newColor.Photo.IsImageOkay(2))
                {
                    ModelState.AddModelError("Photo", "Please choose valid image file");
                    return View(existed);
                }
                
                if(existed.Image != null)
                {
                    FileValidator.FileDelete(_env.WebRootPath, "assets/img/colors", existed.Image);
                    _context.Entry(existed).CurrentValues.SetValues(newColor);
                }
             
                existed.Image = await newColor.Photo.FileCreate(_env.WebRootPath,
                    "assets/img/colors");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Color color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            if (color is null) return NotFound();

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Color existed = _context.Colors.FirstOrDefault(x => x.Id == id);

            if (existed is null) return NotFound();
            return View(existed);
        }
    }
}
