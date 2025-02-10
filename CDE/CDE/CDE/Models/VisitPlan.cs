using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Graph.Models;
using Microsoft.AspNetCore.Identity;

namespace CDE.Models
{
    public class VisitPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string CreatedBy { get; set; } 

        [Required]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(VisitPlan), nameof(ValidateVisitDates))]
        public DateTime VisitStartDate { get; set; }

        public static ValidationResult ValidateVisitDates(DateTime startDate, ValidationContext context)
        {
            var now = DateTime.UtcNow;
            if (startDate < now)
            {
                return new ValidationResult("Visit date cannot be in the past.");
            }
            return ValidationResult.Success;
        }

        [Required]
        public DateTime VisitEndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Required]
        [EnumDataType(typeof(VisitTimeEnum))]
        public VisitTimeEnum VisitTime { get; set; }
        public enum VisitTimeEnum
        {
            [Display(Name = "Morning")]
            Morning,

            [Display(Name = "Afternoon")]
            Afternoon,

            [Display(Name = "Full day")]
            Full_Day    
        }

        [Required]
        [EnumDataType(typeof(VisitStatusEnum))]
        public VisitStatusEnum Status { get; set; }
        public enum VisitStatusEnum
        {
            [Display(Name = "Not Visit")]
            NotVisited,

            [Display(Name = "Visited")]
            Visited,

            [Display(Name = "Evaluated")]
            Evaluated
        }
        public bool IsEvaluated { get; set; } = false;
        public DateTime? EvaluatedAt { get; set; }
        [Required]
        [StringLength(500)]
        public string VisitPurpose { get; set; }

        public virtual ICollection<VisitDistributor> VisitDistributors { get; set; } = new List<VisitDistributor>();
        public virtual ICollection<VisitGuest> VisitGuests { get; set; } = new List<VisitGuest>();
        public virtual ICollection<VisitTask> Tasks { get; set; } = new List<VisitTask>();
    }
}
