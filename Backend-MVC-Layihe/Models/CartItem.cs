using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class CartItem:BaseEntity
    {
        public int ClothesId { get; set; }
        public Clothes Clothes { get; set; }
        public decimal Price { get; set; }
        public byte Quantity { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
