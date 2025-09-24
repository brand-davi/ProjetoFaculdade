using System.ComponentModel.DataAnnotations;

namespace ProjectManagerMvc.Models
{
    public class Team
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(512)]
        public string? Description { get; set; }

        public ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
        public ICollection<TeamUser> TeamUsers { get; set; } = new List<TeamUser>();
    }
}
