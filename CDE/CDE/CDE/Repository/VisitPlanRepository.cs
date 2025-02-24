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
                .FirstOrDefaultAsync(v => v.Id == visitTask.VisitPlanId);

            if (visitPlan == null)
            {
                throw new InvalidOperationException("Visit Plan không tồn tại.");
            }

            List<string> guestList = new List<string>();

            Console.WriteLine($"Dữ liệu GuestList từ DB: {visitPlan.GuestList}");

            try
            {
                string rawGuestList = visitPlan.GuestList;

                Console.WriteLine($"Dữ liệu gốc từ DB: {rawGuestList}");

                // 🔄 **Bước 1: Giải mã lớp JSON đầu tiên**
                var outerJson = JsonSerializer.Deserialize<List<string>>(rawGuestList);

                Console.WriteLine($"Outer JSON sau khi giải mã: {string.Join(", ", outerJson)}");

                // 🔄 **Bước 2: Nếu outerJson chứa JSON lồng nhau, giải mã tiếp**
                if (outerJson != null && outerJson.Count == 1 && outerJson[0].StartsWith("["))
                {
                    guestList = JsonSerializer.Deserialize<List<string>>(outerJson[0]);
                }
                else
                {
                    guestList = outerJson.Select(x => x.Trim('"')).ToList();
                }

                Console.WriteLine($"GuestList sau xử lý: {string.Join(", ", guestList)}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi phân tích GuestList từ database: {ex.Message}");
            }
            // ✅ **Bước 3: Kiểm tra AssigneeId**
            string assigneeId = visitTask.AssigneeId.Trim().Trim('"');
            Console.WriteLine($"AssigneeId cần kiểm tra: {assigneeId}");

            if (!guestList.Contains(assigneeId))
            {
                throw new InvalidOperationException("Người được giao không nằm trong danh sách khách mời.");
            }

            _context.VisitTasks.Add(visitTask);
            await _context.SaveChangesAsync();
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
