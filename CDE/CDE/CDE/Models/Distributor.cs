using System.ComponentModel.DataAnnotations;

namespace CDE.Models
{
    public class Distributor
    {
        [Key]
        public int DistributorId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
    }
}
