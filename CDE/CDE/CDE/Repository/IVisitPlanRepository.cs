using CDE.Models;

namespace CDE.Repository
{
    public interface IVisitPlanRepository
    {
        Task CreateVisitPlanAsync(VisitPlan visitPlan);
        Task UpdateVisitPlanAsync(VisitPlan visitPlan);
        Task DeleteVisitPlanAsync(Guid id);
        Task<IEnumerable<VisitPlan>> SearchVisitPlansAsync(string searchTerm);
        Task NotifyNewVisitRequestAsync(Guid visitPlanId);
        //Task MapRequestToCalendarAsync(Guid visitPlanId);
        //Task ConfirmVisitRequestAsync(Guid visitPlanId);
        //Task<IEnumerable<VisitPlan>> GetVisitRemindersAsync(DateTime reminderDate);
    }
}
