using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CDE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepository _areaService;

        public AreaController(IAreaRepository areaService)
        {
            _areaService = areaService;
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAreas()
        {
            var areas = await _areaService.GetAllAreasAsync();
            return Ok(areas);
        }
        [HttpGet]
        public async Task<IActionResult> GetAreas(string search = "", string sortBy = "", bool ascending = true)
        {
            var areas = await _areaService.GetAreasAsync(search ?? "", sortBy, ascending);
            return Ok(areas);
        }


        // GET: api/Area/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArea(int id)
        {
            var area = await _areaService.GetAreaByIdAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            return Ok(area);
        }

        // POST: api/Area
        [HttpPost]
        public async Task<IActionResult> CreateArea([FromBody] Area area)
        {
            if (area == null)
            {
                return BadRequest("Invalid data.");
            }

            var createdArea = await _areaService.CreateAreaAsync(area);
            return CreatedAtAction(nameof(GetArea), new { id = createdArea.AreaCode }, createdArea);
        }

        // DELETE: api/Area/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(int id)
        {
            var success = await _areaService.DeleteAreaAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

       
    }
}
