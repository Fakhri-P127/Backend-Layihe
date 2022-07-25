using Backend_MVC_Layihe.Models.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Slider:BaseEntity
    {
        public string Image { get; set; }
        [Required(ErrorMessage ="Please enter the title")]
        public string Title { get; set; }
        [Required]
        public string Article { get; set; }
        public string ButtonUrl { get; set; }
        [Required]
        public byte Order { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }

    }
}
