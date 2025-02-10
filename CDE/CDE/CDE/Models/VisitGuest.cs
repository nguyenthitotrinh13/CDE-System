﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class VisitGuest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid VisitPlanId { get; set; } 

        [ForeignKey("VisitPlanId")]
        public virtual VisitPlan VisitPlan { get; set; }

        [Required]
        public string UserId { get; set; }  

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
