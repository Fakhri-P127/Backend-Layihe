using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.Utilities;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Moderator,Admin")]

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

        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
            if(clothes.ColorId == default || clothes.SizeIds is null 
                || clothes.CategoryId is null || clothes.ClothesColorSizeValues is null)
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

            SetRelationBetweenClothes(clothes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {

            if (id is null || id == 0) return NotFound();
            Clothes existed = _context.Clothes.Include(c => c.ClothesImages)
                .Include(c=>c.ClothesColors).ThenInclude(c=>c.ClothesColorSizes)
                .ThenInclude(c=>c.Size).FirstOrDefault(c => c.Id == id);
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
            Clothes existed = _context.Clothes.Include(c => c.ClothesImages).Include(c=>c.ClothesColors)
                .ThenInclude(c=>c.ClothesColorSizes).ThenInclude(c=>c.Size).FirstOrDefault(c => c.Id == id);
            if (existed is null) return NotFound();

            if (!ModelState.IsValid)
            {
                return ErrorMessage("Couldn't load any", existed);
            };

            if (newClothes.ImageIds is null && newClothes.DetailPhotos is null)
            {
                return ErrorMessage("You must upload at least 1 detail image", existed);
            }
            if (newClothes.CategoryId is null || newClothes.ColorId == default 
                || newClothes.SizeIds is null || newClothes.ClothesColorSizeValues is null)
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

            #region clear all color and sizes
            string relations = newClothes.ClothesColorSizeValues.LastOrDefault();
            string[] colorsizeIds = relations.Split(".");
            for (int i = 0; i < colorsizeIds.Length - 1; i++)
            {
                List<string> splittedList = colorsizeIds[i].Split(",").ToList();
                if (splittedList[0] == string.Empty)
                {
                    splittedList.RemoveAt(0);
                }

                splittedList.RemoveAt(splittedList.Count - 1);

                for (int j = 0; j < splittedList.Count; j++)
                {
                    bool resultInt = int.TryParse(splittedList[j], out int csId);
                    if (resultInt)
                    {
                        foreach (var item in existed.ClothesColors.ToList())
                        {
                            item.ClothesColorSizes.RemoveAll(c => c.SizeId == csId);
                            _context.ClothesColors.Remove(item);
                        }
                    }
                }
            }
            #endregion
            
            SetRelationBetweenClothes(newClothes, existed);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int? id)
        {
            if (id is null || id == 0) return NotFound();

            Clothes existed = await _context.Clothes.Include(p => p.ClothesImages)
                .FirstOrDefaultAsync(p => p.Id == id);
            existed.ClothesImages.ForEach(cImage => FileValidator.FileDelete(_env.WebRootPath,
                "assets/img", cImage.Name));

            _context.Clothes.Remove(existed);
            await _context.SaveChangesAsync();
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

        public void SetRelationBetweenClothes(Clothes newClothes,Clothes existed=null)
        {
            // hamisini arraye yighiram, her checkde value elave olunur ve sonuncunu sechirem ki hamisini daxil etdiyini goturum.
            string relations = newClothes.ClothesColorSizeValues.LastOrDefault();
            string[] colorAndSizes = relations.Split(".");
            for (int i = 0; i < colorAndSizes.Length - 1; i++)
            {
                List<string> arrList = colorAndSizes[i].Split(",").ToList();

                // ikinci loopda split edende ilk index boshluq olur onun qarshisini almaq uchun yoxlama apariram.
                if (arrList[0] == string.Empty)
                {
                    arrList.RemoveAt(0);
                }
                ClothesColor clothesColor = default;
                for (int j = 0; j < arrList.Count - 1; j++)
                {
                    bool result = int.TryParse(arrList[j], out int colorAndSizeId);
                    if (result)
                    {
                        if (j == 0)
                        {
                            if(existed is null)
                            {
                                clothesColor = new ClothesColor
                                {
                                    ColorId = colorAndSizeId,
                                    Clothes = newClothes
                                };
                                _context.ClothesColors.Add(clothesColor);
                                continue;
                            }
                            else
                            {
                                clothesColor = new ClothesColor
                                {
                                    ColorId = colorAndSizeId,
                                    Clothes = existed
                                };
                                _context.ClothesColors.Add(clothesColor);
                                continue;
                            }
                           
                        }
                        ClothesColorSize clothesColorSize = new ClothesColorSize
                        {
                            SizeId = colorAndSizeId,
                            ClothesColor = clothesColor
                        };
                        _context.ClothesColorSizes.Add(clothesColorSize);
                    }
                }
            }
        }

    }
}
