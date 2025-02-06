using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CDE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepository _areaService;
        private readonly IDistributorRepository _distributorRepository;

        public AreaController(IAreaRepository areaService, IDistributorRepository distributorRepository)
        {
            _areaService = areaService;
            _distributorRepository = distributorRepository;
        }
        [HttpGet("distributors")]
        public async Task<IActionResult> GetDistributorsByArea([FromQuery] string areaName)
        {
            if (string.IsNullOrEmpty(areaName))
            {
                return BadRequest("Area name is required.");
            }

            var distributors = await _distributorRepository.GetDistributorsByAreaNameAsync(areaName);

            if (distributors == null || distributors.Count == 0)
            {
                return NotFound("No distributors found for the given area.");
            }

            return Ok(distributors);
        }

        [HttpDelete("remove-distributor/{distributorId}")]
        public async Task<IActionResult> RemoveDistributorFromArea(int distributorId)
        {
            if (distributorId <= 0)
            {
                return BadRequest("Invalid distributor ID.");
            }

            var success = await _distributorRepository.DeleteDistributorByIdAsync(distributorId);

            if (!success)
            {
                return NotFound("Distributor not found.");
            }

            return NoContent(); // 204 No Content: successfully deleted
        }
        [HttpPost("add-distributor")]
        public async Task<IActionResult> AddDistributorToArea([FromBody] AddDistributorRequest request)
        {
            var success = await _distributorRepository.AddDistributorToArea(request);
            if (!success) return BadRequest("Invalid AreaId or failed to add distributor.");

            return Ok("Distributor added successfully.");
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string search = "")
        {
            var users = await _areaService.GetUsersAsync(search);

            return Ok(users);
        }
        [HttpPost("assign")]
        public async Task<IActionResult> AssignUserToArea([FromBody] AssignUserRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.UserId) || request.AreaId <= 0)
            {
                return BadRequest("Invalid request.");
            }
            var success = await _areaService.AssignUserToArea(request.UserId, request.AreaId);
            if (!success)
            {
                return NotFound("User or Area not found.");
            }

            return Ok("User assigned successfully.");
        }

        [HttpGet("{areaName}/users")]
        public async Task<IActionResult> GetUsersByArea(string areaName)
        {
            var users = await _areaService.GetUsersByAreaIdAsync(areaName);
            return Ok(users);
        }

        [HttpPost("remove-user")]
        public async Task<IActionResult> RemoveUserFromArea([FromBody] AssignUserRequest request)
        {
            var result = await _areaService.RemoveUserFromArea(request.UserId, request.AreaId);
            if (!result) return BadRequest("User or Area not found.");

            return Ok("User removed successfully.");
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
