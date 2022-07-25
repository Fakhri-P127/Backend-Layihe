using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Setting:BaseEntity
    {
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
