using Backend_MVC_Layihe.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Models
{
    public class Order:BaseEntity
    {
        public decimal TotalPrice { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Address { get; set; }
        public bool? Status { get; set; }
        public List<CartItem> BasketItems { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

    }
}
