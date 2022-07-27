using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Color:BaseEntity
    {
        [Required, StringLength(maximumLength: 25)]
        public string Name { get; set; }
        public List<ClothesColor> ClothesColors { get; set; }


    }
}
