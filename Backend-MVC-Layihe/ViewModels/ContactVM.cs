using Backend_MVC_Layihe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.ViewModels
{
    public class ContactVM
    {
        public List<Setting> Settings { get; set; }
        public Message Message { get; set; }
    }
}
