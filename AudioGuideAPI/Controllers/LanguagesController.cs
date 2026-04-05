using AudioGuideAPI.Database;
using AudioGuideAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LanguagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LanguageDto>>> GetLanguages()
        {
            var result = await _context.Languages
                .AsNoTracking()
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.DisplayName)
                .Select(x => new LanguageDto
                {
                    LanguageCode = x.LanguageCode,
                    DisplayName = x.DisplayName,
                    FlagIcon = x.FlagIcon,
                    IntroText = x.IntroText,
                    IsDefault = x.IsDefault
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}