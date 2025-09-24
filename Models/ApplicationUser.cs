using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagerMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(120)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(11)]
        public string CPF { get; set; } = string.Empty;

        [MaxLength(80)]
        public string? Position { get; set; }

        // Many-to-many Team relation via TeamUser
        public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
    }
}
