using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class TaskRating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [ForeignKey("TaskId")]
        public VisitTask Task { get; set; }

        [Required]
        public string RatedBy { get; set; }

        [ForeignKey("RatedBy")]
        public ApplicationUser Rater { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; } // ⭐ Số sao từ 1 đến 5

        public DateTime RatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
