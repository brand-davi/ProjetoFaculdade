using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetAllAsync();
        Task<TaskItem?> GetByIdAsync(int id);
        Task CreateAsync(TaskItem t);
        Task UpdateAsync(TaskItem t);
        Task DeleteAsync(TaskItem t);
        Task UpdateOwnStatusAsync(int taskId, string userId, ProjectManagerMvc.Models.TaskStatus status);
    }
}
