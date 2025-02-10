using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class VisitDistributor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid VisitPlanId { get; set; }  

        [ForeignKey("VisitPlanId")]
        public virtual VisitPlan VisitPlan { get; set; }

        [Required]
        public int DistributorId { get; set; } 

        [ForeignKey("DistributorId")]
        public virtual Distributor Distributor { get; set; }
    }
}
