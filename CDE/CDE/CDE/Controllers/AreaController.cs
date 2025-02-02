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

        //[HttpPost("create-user")]
        //public async Task<IActionResult> CreateUser([FromBody] User user)
        //{
        //    var newUser = await _areaService.CreateUserAsync(user);
        //    if (newUser == null) return BadRequest("Failed to create user.");

        //    return CreatedAtAction(nameof(GetUsersByArea), new { areaId = newUser.AreaId }, newUser);
        //}

        //[HttpGet("users/{areaId}")]
        //public async Task<IActionResult> GetUsersByArea(int areaId)
        //{
        //    var users = await _areaService.GetUsersByAreaIdAsync(areaId);
        //    return Ok(users);
        //}

        //[HttpGet("users")]
        //public async Task<IActionResult> GetAllUsers()
        //{
        //    var users = await _areaService.GetAllUsersAsync();
        //    return Ok(users);
        //}

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
