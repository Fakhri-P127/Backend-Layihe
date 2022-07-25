using Backend_MVC_Layihe.DAL;
using Backend_MVC_Layihe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_MVC_Layihe.Service
{
    public class LayoutService
    {
        private readonly ApplicationDbContext _context;

        public LayoutService(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Setting> GetSettings()
        {
            List<Setting> settings = _context.Settings.ToList();
            return settings;
        }
    }
}
