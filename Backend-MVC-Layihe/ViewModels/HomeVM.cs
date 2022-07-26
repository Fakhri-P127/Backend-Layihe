using Backend_MVC_Layihe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.ViewModels
{
    public class HomeVM
    {
        public List<Slider> Sliders { get; set; }
        public List<Clothes> Clothes { get; set; }
        public List<SpecialOffer> SpecialOffers { get; set; }
        public List<Category> Categories { get; set; }
    }
}
