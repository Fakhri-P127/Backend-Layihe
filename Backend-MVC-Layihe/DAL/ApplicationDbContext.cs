using Backend_MVC_Layihe.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.DAL
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }

        public DbSet<Clothes> Clothes { get; set; }
        public DbSet<ClothesImage> ClothesImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        //public DbSet<ClothesCategory> ClothesCategories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<ClothesColor> ClothesColors { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<ColorSize> ColorSizes { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<SpecialOffer> SpecialOffers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var item in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetProperties()
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)
                )))
            {
                item.SetColumnType("decimal(6,2)");
            }

            modelBuilder.Entity<Setting>().HasIndex(p => p.Key).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Color>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Size>().HasIndex(p => p.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
