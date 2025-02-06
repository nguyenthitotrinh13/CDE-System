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

        // FK đến bảng AspNetUsers (IdentityUser)
        [Required]
        public Guid ActorId { get; set; }
        [ForeignKey("ActorId")]
        public virtual IdentityUser Actor { get; set; }  // Navigation Property

        [Required]
        public DateTime VisitDate { get; set; }

        [Required]
        [EnumDataType(typeof(VisitTimeEnum))]
        public VisitTimeEnum VisitTime { get; set; }

        // FK đến bảng AspNetUsers (Distributor có thể là một User hoặc một thực thể riêng)
        [Required]
        //public Guid DistributorId { get; set; }
        //[ForeignKey("DistributorId")]
        //public virtual IdentityUser Distributor { get; set; }  

        public string VisitPurpose { get; set; }

        // FK đến danh sách khách mời
        public virtual ICollection<VisitGuest> VisitGuests { get; set; } = new List<VisitGuest>();

        [Required]
        [EnumDataType(typeof(VisitStatusEnum))]
        public VisitStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
