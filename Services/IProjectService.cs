using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Services
{
    public interface IProjectService
    {
        Task<List<Project>> GetAllAsync();
        Task<Project?> GetByIdAsync(int id);
        Task CreateAsync(Project p);
        Task UpdateAsync(Project p);
        Task DeleteAsync(Project p);
    }
}
