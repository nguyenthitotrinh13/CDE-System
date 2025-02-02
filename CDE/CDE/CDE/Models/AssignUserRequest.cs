using Microsoft.AspNetCore.Identity;

namespace CDE.Models
{
    public class AssignUserRequest 
    {
        public string UserId { get; set; }
        public int AreaId { get; set; }
    }
}
