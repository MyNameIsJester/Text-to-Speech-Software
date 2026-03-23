using Microsoft.AspNetCore.Mvc;
using AudioGuideAPI.Data;
using AudioGuideAPI.Models;

namespace AudioGuideAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]                               //POIController → /poi
    public class POIController : ControllerBase
    {
        [HttpGet]                                         //Nghĩa là GET /poi                          
        public ActionResult<List<POI>> GetPOIs()
        {
            var pois = FakeData.GetPOIs();                //Lấy dữ liệu đã tạo
            return Ok(pois);                              //Trả dữ liệu về dạng JSON
        }
    }
}