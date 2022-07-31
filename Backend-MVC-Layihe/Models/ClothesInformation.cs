using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class ClothesInformation:BaseEntity
    {
        [Required]
        public string AdditionalInfo { get; set; }
        [Required,MaxLength(50)]
        public string Definition { get; set; }
        public List<Clothes> Clothes { get; set; }
    }
}
