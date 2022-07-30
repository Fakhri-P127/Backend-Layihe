using Backend_MVC_Layihe.Models.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Clothes : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public string Description { get; set; }
        public string ExtraInfo { get; set; }
        public List<ClothesImage> ClothesImages { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public List<ClothesColor> ClothesColors { get; set; }

        //hele deqiq bilmirem bunlari
        [NotMapped]
        public IFormFile MainPhoto { get; set; }
        [NotMapped]
        public List<IFormFile> DetailPhotos { get; set; }
        [NotMapped]
        public List<int> ImageIds { get; set; }
        [NotMapped]
        public int ColorId { get; set; }
        [NotMapped]
        public List<int> SizeIds { get; set; }
        //[NotMapped]
        //public int SizeId { get; set; }
        //[NotMapped]
        //public byte Quantity { get; set; }

        [NotMapped]
        public List<string> ClothesColorSizeValues { get; set; }
    }
}
