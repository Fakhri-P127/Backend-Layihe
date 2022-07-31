using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class WishlistItem:BaseEntity
    {
        public int ClothesId { get; set; }
        public Clothes Clothes { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
