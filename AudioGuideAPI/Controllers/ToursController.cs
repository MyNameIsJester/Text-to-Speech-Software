using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ToursController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTours()
        {
            try
            {
                var tours = await _context.Tours
                    .Include(t => t.Translations)
                        .ThenInclude(tr => tr.Language)
                    .Include(t => t.TourItems.OrderBy(ti => ti.OrderIndex))
                        .ThenInclude(ti => ti.FoodStall)
                            .ThenInclude(fs => fs.Translations)
                    .Where(t => t.IsActive)
                    .ToListAsync();

                return Ok(tours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi máy chủ nội bộ: {ex.Message}" });
            }
        }
    }
}