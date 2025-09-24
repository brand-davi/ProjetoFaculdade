using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Data;
using ProjectManagerMvc.Models;


namespace ProjectManagerMvc.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _db;
        public TaskService(ApplicationDbContext db) { _db = db; }

        public async Task<List<TaskItem>> GetAllAsync() =>
            await _db.Tasks.Include(t => t.Project).Include(t => t.Assignee)
                .AsNoTracking().OrderBy(t => t.DueDate).ToListAsync();

        public async Task<TaskItem?> GetByIdAsync(int id) =>
            await _db.Tasks.Include(t => t.Project).Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task CreateAsync(TaskItem t)
        {
            _db.Tasks.Add(t);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem t)
        {
            _db.Tasks.Update(t);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem t)
        {
            _db.Tasks.Remove(t);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOwnStatusAsync(int taskId, string userId, ProjectManagerMvc.Models.TaskStatus status)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.AssigneeId == userId);
            if (task == null) throw new UnauthorizedAccessException("Você só pode alterar o status das suas próprias tarefas.");
            task.Status = status;
            await _db.SaveChangesAsync();
        }
    }
}
