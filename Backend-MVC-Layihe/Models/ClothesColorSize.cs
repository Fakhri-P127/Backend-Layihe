using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class ClothesColorSize:BaseEntity
    {
        public int ClothesColorId { get; set; }
        public ClothesColor ClothesColor{ get; set; }
        public int SizeId { get; set; }
        public Size Size { get; set; }
    }
}
