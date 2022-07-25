using Backend_MVC_Layihe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.ViewModels
{
    public class ProductVM
    {
        public Clothes Clothes { get; set; }
        public List<Clothes> Clotheses { get; set; }

    }
}
