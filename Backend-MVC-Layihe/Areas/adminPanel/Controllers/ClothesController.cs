using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Areas.adminPanel.Controllers
{
    [Area("adminpanel")]
    public class ClothesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ClothesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            ViewBag.Categories = _context.Categories.Include(c => c.Clothes).ToList();
            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors)
                   .ThenInclude(c => c.ClothesColorSizes).ThenInclude(c => c.Size).ToList();
            ViewBag.Sizes = _context.Sizes.Include(c => c.ClothesColorSizes)
           .ThenInclude(c => c.ClothesColor).ThenInclude(c => c.Color).ToList();
            ViewBag.ClothesColorSizes = _context.ClothesColorSizes
                .Include(c => c.Size).Include(c => c.ClothesColor).ThenInclude(c=>c.Clothes).ToList();

            List<Clothes> clothes = _context.Clothes.Include(c => c.ClothesImages)
                .Include(c=>c.ClothesColors).ThenInclude(c=>c.ClothesColorSizes)
                .ThenInclude(c=>c.Size).ThenInclude(c=>c.ClothesColorSizes).ThenInclude(c=>c.ClothesColor).ThenInclude(c=>c.Color)
                .Include(c=>c.Category)
                .ToList();

            return View(clothes);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.Include(c=>c.Clothes).ToList();
            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors)
                   .ThenInclude(c => c.ClothesColorSizes).ThenInclude(c => c.Size).ToList();
            ViewBag.Sizes = _context.Sizes.Include(c => c.ClothesColorSizes)
                .ThenInclude(c => c.ClothesColor).ThenInclude(c => c.Color).ToList();
            ViewBag.ClothesColorSizes = _context.ClothesColorSizes
                .Include(c => c.Size).Include(c => c.ClothesColor).ThenInclude(c => c.Clothes).ToList();
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(Clothes clothes)
        {
            if (!ModelState.IsValid)
            {
                return ErrorMessage(string.Empty);
            } 

            if (clothes.MainPhoto is null || clothes.DetailPhotos is null)
            {
                return ErrorMessage("Choose at least 1 main image and 1 detail image");
            }

            if (!clothes.MainPhoto.IsImageOkay(2))
            {
                return ErrorMessage("Please choose valid image file");
            }
            if(clothes.ColorIds is null || clothes.SizeIds is null || clothes.CategoryId is null)
            {
                return ErrorMessage("Choose at least 1 size, 1 color and 1 category");
            }

            ClothesImage main = new ClothesImage
            {
                Name = await clothes.MainPhoto.FileCreate(_env.WebRootPath,
                "assets/img", null, _context.ClothesImages),
                IsMain = true,
                Alternative = clothes.Name,
                Clothes = clothes
            };
            await _context.ClothesImages.AddAsync(main);

            TempData["FileName"] = default(string);
          
            foreach (IFormFile photo in clothes.DetailPhotos.ToList())
            {
                if (!photo.IsImageOkay(2))
                {
                    clothes.DetailPhotos.Remove(photo);
                    TempData["FileName"] += $"{photo.FileName},";
                    continue;
                }
                ClothesImage photoImage = new ClothesImage
                {
                    Name = await photo.FileCreate(_env.WebRootPath,
                    "assets/img"),
                    IsMain = false,
                    Alternative = clothes.Name,
                    Clothes = clothes,
                };
                _context.ClothesImages.Add(photoImage);
            }
            // if all the photos are removed
            if (clothes.DetailPhotos.Count == 0)
            {
                return ErrorMessage("Couldn't load any of the detail images");
            }
      
            foreach (int colorId in clothes.ColorIds)
            {
                ClothesColor clothesColor = new ClothesColor
                {
                    ColorId = colorId,
                    Clothes = clothes,
                };
                _context.ClothesColors.Add(clothesColor);

                foreach (int sizeId in clothes.SizeIds)
                {
                    ClothesColorSize clothesColorSize = new ClothesColorSize
                    {
                        SizeId = sizeId,
                        ClothesColor = clothesColor
                    };
                    _context.ClothesColorSizes.Add(clothesColorSize);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {

            if (id is null || id == 0) return NotFound();
            Clothes existed = _context.Clothes.Include(c => c.ClothesImages).FirstOrDefault(c => c.Id == id);
            if (existed is null) return NotFound();

            ViewBag.Categories = _context.Categories.Include(c => c.Clothes).ToList();
            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors)
                   .ThenInclude(c => c.ClothesColorSizes).ThenInclude(c => c.Size).ToList();
            ViewBag.Sizes = _context.Sizes.Include(c => c.ClothesColorSizes)
                .ThenInclude(c => c.ClothesColor).ThenInclude(c => c.Color).ToList();
            ViewBag.ClothesColorSizes = _context.ClothesColorSizes
                .Include(c => c.Size).Include(c => c.ClothesColor).ThenInclude(c => c.Clothes).ToList();

            return View(existed);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(int? id, Clothes newClothes)
        {
            Clothes existed = _context.Clothes.Include(c => c.ClothesImages).FirstOrDefault(c => c.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return ErrorMessage("Couldn't load any", existed);
            };

            if (newClothes.ImageIds is null && newClothes.DetailPhotos is null)
            {
                return ErrorMessage("You must upload at least 1 detail image", existed);
            }
            if (newClothes.CategoryId is null || newClothes.ColorIds is null || newClothes.SizeIds is null)
            {
                return ErrorMessage("You must choose at least 1 category, 1 color and 1 size", existed);
            }

            _context.Entry(existed).CurrentValues.SetValues(newClothes);


            TempData["FileName"] = default(string);
            //if the user inputs detail photos
            if (newClothes.DetailPhotos != null)
            {
                // butun detail shekillerini silib bashqalarini elave etmek uchundu
                if (newClothes.ImageIds is null)
                {
                    foreach (ClothesImage dtlImage in existed.ClothesImages
                        .Where(p => p.IsMain == false))
                    {
                        FileValidator.FileDelete(_env.WebRootPath,
                            "assets/img", dtlImage.Name);
                    }
                    existed.ClothesImages.RemoveAll(p => p.IsMain == false);
                }
                else
                {
                    List<ClothesImage> removableImages = existed.ClothesImages
                  .Where(p => p.IsMain == false && !newClothes.ImageIds
                  .Contains(p.Id)).ToList();

                    existed.ClothesImages
                        .RemoveAll(p => removableImages.Any(r => p.Id == r.Id));

                    removableImages.ForEach(r => FileValidator.FileDelete(_env.WebRootPath,
                        "assets/img", r.Name));

                }

                foreach (IFormFile image in newClothes.DetailPhotos.ToList())
                {
                    //butun upload olunmush shekiller shertlerimize uymursa
                    if (newClothes.DetailPhotos.Count == 0)
                    {
                        return ErrorMessage(
                            "None of the detail images are valid type", existed);
                    }
                    // burda yuklediyimiz bir neche shekilden her hansisa problemli olsa silsin deyirik. Bir iki shekile gore butun shekiller silinmesin
                    if (!image.IsImageOkay(2))
                    {
                        newClothes.DetailPhotos.Remove(image);
                        TempData["FileName"] += image.FileName + ",";
                        continue;
                    }

                    ClothesImage detailImage = new ClothesImage
                    {
                        Name = await image.FileCreate(_env.WebRootPath,
                        "assets/img", null, _context.ClothesImages),
                        IsMain = false,
                        Clothes = existed,
                        Alternative = newClothes.Name,
                    };
                    await _context.ClothesImages.AddAsync(detailImage);
                }
            }

            //if the user inputs main photo
            if (newClothes.MainPhoto != null)
            {
                if (!newClothes.MainPhoto.IsImageOkay(2))
                {
                    return ErrorMessage("Please choose valid image file", existed);
                }
                ClothesImage main = new ClothesImage
                {
                    IsMain = true,
                    Alternative = newClothes.Name,
                    Name = await newClothes.MainPhoto
                    .FileCreate(_env.WebRootPath,
                    "assets/img", null, _context.ClothesImages),
                    Clothes = existed,
                };
                string mainPhoto = existed.ClothesImages
                    .FirstOrDefault(p => p.IsMain == true).Name;
                FileValidator.FileDelete(_env.WebRootPath,
                    "assets/img", mainPhoto);
                //name ve alternative i yaratdigimiza menimsedirik ki yeni photo ile override edek
                existed.ClothesImages.FirstOrDefault(p => p.IsMain == true).Name = main.Name;
                existed.ClothesImages.FirstOrDefault(p => p.IsMain == true)
                    .Alternative = main.Alternative;
            }

        

            // Editing Category starts
            //removing categories that doesn't match with CategoryIds
            //List<PlantCategory> removableCategories = existed.PlantCategories
            //    .Where(p => !newClothes.CategoryIds.Contains(p.CategoryId)).ToList();

            //existed.PlantCategories.RemoveAll(c => removableCategories
            //.Any(r => r.CategoryId == c.CategoryId));

            ////adding categories that don't contain CategoryIds
            //newClothes.CategoryIds.ForEach(async ctgId =>
            //{
            //    PlantCategory currCategory = existed.PlantCategories
            //    .FirstOrDefault(p => p.CategoryId == ctgId);

            //    if (!existed.PlantCategories.Contains(currCategory))
            //    {
            //        PlantCategory category = new PlantCategory
            //        {
            //            CategoryId = ctgId,
            //            Plant = existed
            //        };
            //        await _context.PlantCategories.AddAsync(category);
            //    }
            //});
            // Editing Category ends

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        // Utility methods
        public IActionResult ErrorMessage(string text, Clothes existed = null)
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Colors = _context.Colors.Include(c => c.ClothesColors)
                .ThenInclude(c => c.ClothesColorSizes).ThenInclude(c => c.Size).ToList();
            ViewBag.Sizes = _context.Sizes.Include(c => c.ClothesColorSizes)
           .ThenInclude(c => c.ClothesColor).ThenInclude(c => c.Color).ToList();
            ViewBag.ClothesColorSizes = _context.ClothesColorSizes
            .Include(c => c.Size).Include(c => c.ClothesColor).ThenInclude(c => c.Clothes).ToList();
            ModelState.AddModelError(string.Empty, text);
            return View(existed);
        }

    }
}
