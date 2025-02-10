using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class TaskAttachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        [ForeignKey("TaskId")]
        public VisitTask Task { get; set; }

        [Required]
        [MaxLength(500)]
        public string FileName { get; set; } // Tên file

        [Required]
        [MaxLength(1000)]
        public string FilePath { get; set; } 

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public string? UploadedBy { get; set; }

        [ForeignKey("UploadedBy")]
        public ApplicationUser UploadedUser { get; set; }
    }
}
