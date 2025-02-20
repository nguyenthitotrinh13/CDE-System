using CDE.DBContexts;
using CDE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static CDE.Models.VisitPlan;
using System.Text.Json;

namespace CDE.Repository
{
    public class VisitPlanRepository : IVisitPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateTaskAsync(VisitTask visitTask)
        {
            var visitPlan = await _context.VisitPlans
                .Include(v => v.Tasks)
                .FirstOrDefaultAsync(v => v.Id == visitTask.VisitPlanId);

            if (visitPlan == null)
            {
                throw new InvalidOperationException("Visit Plan không tồn tại.");
            }

            Console.WriteLine($"GuestList JSON: {visitPlan.GuestList}");

            var guestListRaw = JsonSerializer.Deserialize<List<string>>(visitPlan.GuestList);

            var guestList = guestListRaw
                .SelectMany(raw =>
                {
                    try
                    {
                        return JsonSerializer.Deserialize<List<string>>(raw);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON Decode Error: {ex.Message}");
                        return new List<string>();
                    }
                })
                .Select(g => g.Trim('"'))
                .ToList();

            if (!guestList.Contains(visitTask.AssigneeId))
            {
                throw new InvalidOperationException("Người được giao không nằm trong danh sách khách mời.");
            }

            if (visitTask.StartDate < visitPlan.VisitStartDate || visitTask.EndDate < visitTask.StartDate)
            {
                throw new InvalidOperationException("Ngày bắt đầu hoặc hạn chót không hợp lệ.");
            }

            _context.VisitTasks.Add(visitTask);
            await _context.SaveChangesAsync();

            Console.WriteLine("Task đã được tạo thành công!");
        }
        

        public async Task UpdateVisitPlanAsync(VisitPlan visitPlan)
        {
            _context.VisitPlans.Update(visitPlan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVisitPlanAsync(Guid id)
        {
            var visitPlan = await _context.VisitPlans.FindAsync(id);
            if (visitPlan != null)
            {
                _context.VisitPlans.Remove(visitPlan);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm theo yêu cầu
        public async Task<IEnumerable<VisitPlan>> SearchVisitPlansAsync(string searchTerm)
        {
            return await _context.VisitPlans
                .Where(v => v.VisitPurpose.Contains(searchTerm) || v.CreatedBy.Contains(searchTerm))
                .ToListAsync();
        }

        // Thông báo yêu cầu mới
        public async Task NotifyNewVisitRequestAsync(Guid visitPlanId)
        {
            var visitPlan = await _context.VisitPlans.FindAsync(visitPlanId);
            if (visitPlan != null)
            {
                await SendNotificationAsync(visitPlan.CreatedBy, "New visit request created", visitPlan.VisitPurpose);
            }
        }

        private async Task SendNotificationAsync(string recipient, string subject, string message)
        {
            // Gửi thông báo tới người nhận (ví dụ: qua Email)
            await Task.CompletedTask;
        }

        public Task CreateVisitPlanAsync(VisitPlan visitPlan)
        {
            throw new NotImplementedException();
        }
    }
}
