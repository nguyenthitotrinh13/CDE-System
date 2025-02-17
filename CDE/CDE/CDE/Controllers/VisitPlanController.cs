using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CDE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitPlanController : ControllerBase
    {
        private readonly IVisitPlanRepository _service;

        public VisitPlanController(IVisitPlanRepository service)
        {
            _service = service;
        }

        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask([FromBody] VisitTask task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.CreateTaskAsync(task);
                return Ok(new { message = "Tạo nhiệm vụ thành công.", taskId = task.Id });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateVisitPlan([FromBody] VisitPlan visitPlan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (visitPlan == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                if (visitPlan.GuestList != null)
                {
                    visitPlan.GuestList = JsonConvert.SerializeObject(visitPlan.GuestList.Split(',').ToList());
                }

                if (visitPlan.DistributorList != null)
                {
                    visitPlan.DistributorList = JsonConvert.SerializeObject(visitPlan.DistributorList.Split(',').ToList());
                }

                await _service.CreateVisitPlanAsync(visitPlan);
                return Ok(new { message = "Tạo lịch viếng thăm thành công." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVisitPlan(Guid id, [FromBody] VisitPlan visitPlan)
        {
            if (id != visitPlan.Id)
            {
                return BadRequest();
            }

            await _service.UpdateVisitPlanAsync(visitPlan);
            return NoContent();
        }

        // Delete visit plan
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisitPlan(Guid id)
        {
            await _service.DeleteVisitPlanAsync(id);
            return NoContent();
        }
        [HttpPost("search")]
        public async Task<IActionResult> SearchVisitPlans([FromBody] string searchTerm)
        {
            var visitPlans = await _service.SearchVisitPlansAsync(searchTerm);
            return Ok(visitPlans);
        }

        [HttpPost("notify/{id}")]
        public async Task<IActionResult> NotifyNewVisitRequest(Guid id)
        {
            await _service.NotifyNewVisitRequestAsync(id);
            return NoContent();
        }

        //[HttpPost("map/{id}")]
        //public async Task<IActionResult> MapRequestToCalendar(Guid id)
        //{
        //    await _service.MapRequestToCalendarAsync(id);
        //    return NoContent();
        //}

        //[HttpPost("confirm/{id}")]
        //public async Task<IActionResult> ConfirmVisitRequest(Guid id)
        //{
        //    await _service.ConfirmVisitRequestAsync(id);
        //    return NoContent();
        //}

        //[HttpGet("reminders/{date}")]
        //public async Task<IActionResult> GetVisitReminders(DateTime date)
        //{
        //    var reminders = await _service.GetVisitRemindersAsync(date);
        //    return Ok(reminders);
        //}
    }
}
