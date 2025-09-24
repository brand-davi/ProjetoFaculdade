using System.ComponentModel.DataAnnotations;

namespace ProjectManagerMvc.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(160)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public Project? Project { get; set; }

        public string? AssigneeId { get; set; }
        public ApplicationUser? Assignee { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
}
