﻿using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using Backend_MVC_Layihe.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Detail(int? id)
        {
            if (id is null || id == 0) return NotFound();

            ProductVM model = new ProductVM
            {
                Clothes = _context.Clothes
                .Include(c => c.ClothesImages)
                .Include(c => c.ClothesColors).ThenInclude(c => c.Color)
                //.ThenInclude(c => c.ColorSizes).ThenInclude(c => c.Size)
                .FirstOrDefault(c => c.Id == id),
                Clotheses = new List<Clothes>(),
                Category = _context.Clothes.Include(c=>c.Category).FirstOrDefault(c=>c.Id==id).Category

            };
            List<Clothes> clothes = new List<Clothes>();

            foreach (Clothes product in model.Category.Clothes.ToList())
            {
                clothes = _context.Clothes.Include(x => x.Category).ThenInclude(c => c.Clothes)
                    .Include(x => x.ClothesImages)
                    .Where(p => product.CategoryId == p.CategoryId && p.Id !=id).ToList();
                    //.Any(x => x.CategoryId == product.CategoryId);
                    
                    //&& p.CategoryId != product.CategoryId).ToList();
                model.Clotheses.AddRange(clothes);
            }
            model.Clotheses = model.Clotheses.Distinct().ToList();
            
            if (model.Clothes is null) return NotFound();

            return View(model);
        }

    }
}
