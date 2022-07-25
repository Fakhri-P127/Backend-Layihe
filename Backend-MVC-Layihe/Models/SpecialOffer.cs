using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class SpecialOffer:BaseEntity
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Discount { get; set; }
        public string ButtonUrl { get; set; }

    }
}
