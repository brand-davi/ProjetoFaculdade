namespace ProjectManagerMvc.Models
{
    public class ProjectTeam
    {
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public int TeamId { get; set; }
        public Team? Team { get; set; }
    }

    public class TeamUser
    {
        public int TeamId { get; set; }
        public Team? Team { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
