using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CDE.Models
{
    [Table("Tasks")]
    public class VisitTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }  

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }  

        [Required]
        public Guid VisitPlanId { get; set; }

        [ForeignKey(nameof(VisitPlanId))]
        [JsonIgnore]
        public VisitPlan? VisitPlan { get; set; }

        [Required]
        public string AssigneeId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }  

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();

        public virtual ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
        public virtual ICollection<TaskRating> Ratings { get; set; } = new List<TaskRating>();

    }
}
