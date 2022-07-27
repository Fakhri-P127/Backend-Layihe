using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class ClothesColor:BaseEntity
    {
        public int ClothesId { get; set; }
        public Clothes Clothes { get; set; }
        public int ColorId { get; set; }
        public Color Color { get; set; }
        public List<ClothesColorSize> ClothesColorSizes { get; set; }

    }
}
