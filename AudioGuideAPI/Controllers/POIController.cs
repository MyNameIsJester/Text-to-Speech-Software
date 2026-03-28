using AudioGuideAPI.Database;
using AudioGuideAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AudioGuideAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PoiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PoiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/poi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<POI>>> GetPOIs()
        {
            return await _context.POIs.ToListAsync();
        }

        // GET: api/poi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<POI>> GetPOI(int id)
        {
            var poi = await _context.POIs.FindAsync(id);

            if (poi == null)
                return NotFound();

            return poi;
        }

        // POST: api/poi
        [HttpPost]
        public async Task<ActionResult<POI>> CreatePOI(POI poi)
        {
            _context.POIs.Add(poi);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPOI), new { id = poi.Id }, poi);
        }

        // PUT: api/poi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePOI(int id, POI poi)
        {
            if (id != poi.Id)
                return BadRequest();

            _context.Entry(poi).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.POIs.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/poi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePOI(int id)
        {
            var poi = await _context.POIs.FindAsync(id);

            if (poi == null)
                return NotFound();

            _context.POIs.Remove(poi);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}