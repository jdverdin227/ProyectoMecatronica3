using Microsoft.AspNetCore.Identity;
using Proyecto.Models.Enums;

namespace Proyecto.Models
{
    public class User : IdentityUser
    {
        // Additional properties can be added here if needed
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? FullName { get; set; }
        public UserType UserType { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiryTime { get; set; }  = null;


    }
}
