using System.ComponentModel.DataAnnotations;

namespace ProjectManagerMvc.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1024)]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

        [DataType(DataType.Date)]
        public DateTime? PlannedEndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

        // Manager (User)
        [Required]
        public string ManagerId { get; set; } = string.Empty;
        public ApplicationUser? Manager { get; set; }

        // Relations
        public ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
