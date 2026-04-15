using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAdmin.Controllers
{
    public class TestDbController : Controller
    {
        private readonly AppDbContext _context;

        public TestDbController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var foodStallCount = await _context.FoodStalls.CountAsync();
            return Content($"DB connection OK. FoodStalls count = {foodStallCount}");
        }
    }
}