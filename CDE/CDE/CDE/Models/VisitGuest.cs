using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class VisitGuest
    {
        [Key]
        public Guid Id { get; set; }  

        [Required]
        public Guid VisitScheduleId { get; set; }
        [ForeignKey("VisitPlanId")]
        public virtual VisitPlan VisitPlan { get; set; }

        // FK đến bảng AspNetUsers (IdentityUser)
        [Required]
        public Guid GuestId { get; set; }
        [ForeignKey("GuestId")]
        public virtual IdentityUser Guest { get; set; }  // Người được mời
    }
}
