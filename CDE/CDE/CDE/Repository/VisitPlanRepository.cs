using CDE.DBContexts;
using CDE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using static CDE.Models.VisitPlan;

namespace CDE.Repository
{
    public class VisitPlanRepository : IVisitPlanRepository
    {
        private readonly ApplicationDbContext _context;

        public VisitPlanRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task CreateVisitPlanAsync(VisitPlan visitPlan)
        {
            var today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Date;
            var existingPlans = await _context.VisitPlans
                .Where(v => v.CreatedBy == visitPlan.CreatedBy && v.VisitStartDate.Date == today)
                .ToListAsync();

            if (existingPlans.Count >= 2)
            {
                throw new InvalidOperationException("Bạn chỉ có thể gửi tối đa 2 yêu cầu viếng thăm trong một ngày.");
            }

            bool hasMorningRequest = existingPlans.Any(v => v.VisitTime == VisitPlan.VisitTimeEnum.Morning);
            bool hasAfternoonRequest = existingPlans.Any(v => v.VisitTime == VisitPlan.VisitTimeEnum.Afternoon);

            if ((visitPlan.VisitTime == VisitPlan.VisitTimeEnum.Morning && hasMorningRequest) ||
                (visitPlan.VisitTime == VisitPlan.VisitTimeEnum.Afternoon && hasAfternoonRequest))
            {
                throw new InvalidOperationException("Bạn không thể gửi 2 yêu cầu vào cùng một khoảng thời gian (sáng hoặc chiều).");
            }

            if (visitPlan.VisitStartDate.Date == today)
            {
                var lastVisit = existingPlans.OrderByDescending(v => v.VisitEndDate).FirstOrDefault();
                if (lastVisit != null && (visitPlan.VisitStartDate - lastVisit.VisitEndDate).TotalMinutes < 60)
                {
                    throw new InvalidOperationException("Lịch viếng thăm phải cách ít nhất 1 giờ so với lịch trước.");
                }
            }

            if (!string.IsNullOrEmpty(visitPlan.GuestList))
            {
                visitPlan.GuestList = JsonConvert.SerializeObject(visitPlan.GuestList.Split(',').ToList());
            }

            if (!string.IsNullOrEmpty(visitPlan.DistributorList))
            {
                visitPlan.DistributorList = JsonConvert.SerializeObject(visitPlan.DistributorList.Split(',').ToList());
            }

            _context.VisitPlans.Add(visitPlan);
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
        //private readonly ApplicationDbContext _context;
        //private readonly IVisitPlanRepository _visitPlanRepository;

        //public VisitPlanRepository(IVisitPlanRepository visitPlanRepository)
        //{
        //    _visitPlanRepository = visitPlanRepository;
        //}
        //public VisitPlanRepository(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //public async Task<VisitPlan> GetVisitPlanByIdAsync(Guid id)
        //{
        //    return await _context.VisitPlans
        //        .Include(v => v.VisitDistributors)
        //        .Include(v => v.VisitGuests)
        //        .FirstOrDefaultAsync(v => v.Id == id);
        //}

        //public async Task<IEnumerable<VisitPlan>> GetAllVisitPlansAsync()
        //{
        //    return await _context.VisitPlans
        //        .Include(v => v.VisitDistributors)
        //        .Include(v => v.VisitGuests)
        //        .ToListAsync();
        //}

        ////public async Task CreateVisitPlanAsync(VisitPlan visitPlan) //người tạo chỉ có thể gửi tối đa 2 visit request, nếu là ngày hiện tại phải cách 1h
        ////{
        ////    var today = DateTime.UtcNow.Date;
        ////    var existingPlans = await _context.VisitPlans
        ////        .Where(v => v.CreatedBy == visitPlan.CreatedBy && v.VisitStartDate.Date == today)
        ////        .ToListAsync();

        ////    // Kiểm tra nếu đã có 2 visit request trong ngày
        ////    if (existingPlans.Count >= 2)
        ////    {
        ////        throw new InvalidOperationException("Bạn chỉ có thể gửi tối đa 2 yêu cầu viếng thăm trong một ngày.");
        ////    }

        ////    // Kiểm tra nếu đã có yêu cầu buổi sáng hoặc buổi chiều
        ////    bool hasMorningRequest = existingPlans.Any(v => v.VisitTime == VisitPlan.VisitTimeEnum.Morning);
        ////    bool hasAfternoonRequest = existingPlans.Any(v => v.VisitTime == VisitPlan.VisitTimeEnum.Afternoon);

        ////    if ((visitPlan.VisitTime == VisitPlan.VisitTimeEnum.Morning && hasMorningRequest) ||
        ////        (visitPlan.VisitTime == VisitPlan.VisitTimeEnum.Afternoon && hasAfternoonRequest))
        ////    {
        ////        throw new InvalidOperationException("Bạn không thể gửi 2 yêu cầu vào cùng một khoảng thời gian (sáng hoặc chiều).");
        ////    }

        ////    // Nếu tạo lịch viếng thăm trong ngày hôm nay, kiểm tra khoảng cách 1 giờ
        ////    if (visitPlan.VisitStartDate.Date == today)
        ////    {
        ////        var lastVisit = existingPlans.OrderByDescending(v => v.VisitEndDate).FirstOrDefault();
        ////        if (lastVisit != null && (visitPlan.VisitStartDate - lastVisit.VisitEndDate).TotalMinutes < 60)
        ////        {
        ////            throw new InvalidOperationException("Lịch viếng thăm phải cách ít nhất 1 giờ so với lịch trước.");
        ////        }

        ////    }

        ////    _context.VisitPlans.Add(visitPlan);
        ////    await _context.SaveChangesAsync();
        ////}
        //public async Task CreateVisitPlanAsync(VisitPlan visitPlan)
        //{
        //    var today = DateTime.UtcNow.Date;
        //    var existingPlans = await _context.VisitPlans
        //        .Where(v => v.CreatedBy == visitPlan.CreatedBy && v.VisitStartDate.Date == today)
        //        .ToListAsync();

        //    // Kiểm tra nếu đã có 2 visit request trong ngày
        //    if (existingPlans.Count >= 2)
        //    {
        //        throw new InvalidOperationException("Bạn chỉ có thể gửi tối đa 2 yêu cầu viếng thăm trong một ngày.");
        //    }

        //    // Kiểm tra nếu đã có yêu cầu buổi sáng hoặc buổi chiều
        //    bool hasMorningRequest = existingPlans.Any(v => v.VisitTime == VisitPlan.VisitTimeEnum.Morning);
        //    bool hasAfternoonRequest = existingPlans.Any(v => v.VisitTime == VisitPlan.VisitTimeEnum.Afternoon);

        //    if ((visitPlan.VisitTime == VisitPlan.VisitTimeEnum.Morning && hasMorningRequest) ||
        //        (visitPlan.VisitTime == VisitPlan.VisitTimeEnum.Afternoon && hasAfternoonRequest))
        //    {
        //        throw new InvalidOperationException("Bạn không thể gửi 2 yêu cầu vào cùng một khoảng thời gian (sáng hoặc chiều).");
        //    }

        //    // Nếu tạo lịch viếng thăm trong ngày hôm nay, kiểm tra khoảng cách 1 giờ
        //    if (visitPlan.VisitStartDate.Date == today)
        //    {
        //        var lastVisit = existingPlans.OrderByDescending(v => v.VisitEndDate).FirstOrDefault();
        //        if (lastVisit != null && (visitPlan.VisitStartDate - lastVisit.VisitEndDate).TotalMinutes < 60)
        //        {
        //            throw new InvalidOperationException("Lịch viếng thăm phải cách ít nhất 1 giờ so với lịch trước.");
        //        }
        //    }

        //    // Lưu VisitPlan vào cơ sở dữ liệu
        //    _context.VisitPlans.Add(visitPlan);
        //    await _context.SaveChangesAsync();  // Lưu VisitPlan trước, lấy ID của nó

        //    // Lưu các VisitDistributors
        //    foreach (var distributorId in visitPlan.VisitDistributors)
        //    {
        //        var visitDistributor = new VisitDistributor
        //        {
        //            VisitPlanId = visitPlan.Id,
        //            DistributorId = distributorId.DistributorId
        //        };
        //        _context.VisitDistributors.Add(visitDistributor);
        //    }

        //    // Lưu các VisitGuests
        //    foreach (var guestId in visitPlan.VisitGuests)
        //    {
        //        var visitGuest = new VisitGuest
        //        {
        //            VisitPlanId = visitPlan.Id,
        //            UserId = guestId.UserId
        //        };
        //        _context.VisitGuests.Add(visitGuest);
        //    }

        //    // Lưu vào cơ sở dữ liệu
        //    await _context.SaveChangesAsync();
        //}

        //public async Task UpdateVisitPlanAsync(VisitPlan visitPlan) //update trước 12h, update sẽ phải gửi remind 
        //{
        //    _context.VisitPlans.Update(visitPlan);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task DeleteVisitPlanAsync(Guid id) //sau thời điểm bắt đầu lịch, ko thể delete
        //{
        //    var visitPlan = await _context.VisitPlans.FindAsync(id);
        //    if (visitPlan != null)
        //    {
        //        _context.VisitPlans.Remove(visitPlan);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        //// Tìm kiếm theo yêu cầu
        //public async Task<IEnumerable<VisitPlan>> SearchVisitPlansAsync(string searchTerm)
        //{
        //    return await _context.VisitPlans
        //        .Where(v => v.VisitPurpose.Contains(searchTerm) || v.CreatedBy.Contains(searchTerm))
        //        .ToListAsync();
        //}

        //// Thông báo yêu cầu mới
        //public async Task NotifyNewVisitRequestAsync(Guid visitPlanId)
        //{
        //    var visitPlan = await _context.VisitPlans.FindAsync(visitPlanId);
        //    if (visitPlan != null)
        //    {
        //        // Gửi thông báo tới actor hoặc hệ thống (có thể sử dụng Email, SMS, hoặc dịch vụ push notification)
        //        // Ví dụ: Gửi thông báo qua Email hoặc hệ thống thông báo (giả định đã có phương thức gửi thông báo)
        //        await SendNotificationAsync(visitPlan.CreatedBy, "New visit request created", visitPlan.VisitPurpose);
        //    }
        //}

        //// Ánh xạ yêu cầu vào lịch
        //public async Task MapRequestToCalendarAsync(Guid visitPlanId)
        //{
        //    var visitPlan = await _context.VisitPlans.FindAsync(visitPlanId);
        //    if (visitPlan != null)
        //    {
        //        // Ánh xạ kế hoạch viếng thăm vào lịch (ví dụ: Google Calendar, Outlook Calendar)
        //        // Giả định có một phương thức để ánh xạ vào lịch
        //        await MapToCalendar(visitPlan);
        //    }
        //}

        //// Xác nhận yêu cầu viếng thăm
        //public async Task ConfirmVisitRequestAsync(Guid visitPlanId)
        //{
        //    var visitPlan = await _context.VisitPlans.FindAsync(visitPlanId);
        //    if (visitPlan != null)
        //    {
        //        visitPlan.Status = VisitPlan.VisitStatusEnum.Visited;
        //        await _context.SaveChangesAsync();
        //    }
        //}

        //// Hiển thị nhắc nhở viếng thăm
        //public async Task<IEnumerable<VisitPlan>> GetVisitRemindersAsync(DateTime reminderDate)
        //{
        //    return await _context.VisitPlans
        //        .Where(v => v.VisitStartDate.Date == reminderDate.Date && v.Status == VisitPlan.VisitStatusEnum.NotVisited)
        //        .ToListAsync();
        //}

        //private async Task SendNotificationAsync(string recipient, string subject, string message)
        //{
        //    // Gửi thông báo tới người nhận (ví dụ: qua Email)
        //    await Task.CompletedTask;
        //}

        //private async Task MapToCalendar(VisitPlan visitPlan)
        //{
        //    // Ánh xạ kế hoạch vào một dịch vụ lịch (Google Calendar, Outlook, v.v.)
        //    await Task.CompletedTask;
        //}
    }
}
