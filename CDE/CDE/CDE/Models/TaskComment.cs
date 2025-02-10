using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class TaskComment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [ForeignKey("TaskId")]
        public VisitTask Task { get; set; }

        [Required]
        [MaxLength(1000)]
        public string CommentText { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Required]
        public string CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public ApplicationUser Creator { get; set; }
    }
}
