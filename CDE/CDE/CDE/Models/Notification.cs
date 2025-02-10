using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; } // Người nhận thông báo
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public bool IsRead { get; set; } = false;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}
