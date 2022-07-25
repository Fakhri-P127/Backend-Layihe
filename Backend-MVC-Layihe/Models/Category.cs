using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Category : BaseEntity
    {
        [Required, StringLength(maximumLength: 20)]
        public string Name { get; set; }
        public string Image { get; set; }
        public List<ClothesCategory> ClothesCategories { get; set; }

    }
}
