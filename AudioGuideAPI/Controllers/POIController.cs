using Microsoft.AspNetCore.Mvc;
using AudioGuideAPI.Data;
using AudioGuideAPI.Models;

namespace AudioGuideAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]                         //endoint to /api/poi
    public class POIController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<POI>> Get()         //Danh sách POI
        {
            var pois = FakeData.GetPOIs();
            return Ok(pois);
        }
    }
}