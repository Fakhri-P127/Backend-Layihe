using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Setting:BaseEntity
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }

    }
}
