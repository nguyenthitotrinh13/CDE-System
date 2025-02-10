﻿using CDE.Models;
using CDE.Repository;
using Microsoft.AspNetCore.Mvc;

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

        // Get all visit plans
        [HttpGet]
        public async Task<IActionResult> GetAllVisitPlans()
        {
            var visitPlans = await _service.GetAllVisitPlansAsync();
            return Ok(visitPlans);
        }

        // Get visit plan by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVisitPlanById(Guid id)
        {
            var visitPlan = await _service.GetVisitPlanByIdAsync(id);
            if (visitPlan == null)
            {
                return NotFound();
            }
            return Ok(visitPlan);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVisitPlan([FromBody] VisitPlan visitPlan)
        {
            if (visitPlan == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            await _service.CreateVisitPlanAsync(visitPlan);
            return Ok("Tạo lịch viếng thăm thành công.");
        }


        // Update visit plan
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
