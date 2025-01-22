using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class Area
    {
        [Key]
        public int AreaCode { get; set; }

        [Required]
        [StringLength(100)]
        public string AreaName { get; set; }

        public int? DistributorCity { get; set; }

        [ForeignKey("User")]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [ForeignKey("Distributor")]
        public int? DistributorId { get; set; }
        public Distributor? Distributor { get; set; }
    }
}
