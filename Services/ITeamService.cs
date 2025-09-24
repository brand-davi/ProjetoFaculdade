using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Services
{
    public interface ITeamService
    {
        Task<List<Team>> GetAllAsync();
        Task<Team?> GetByIdAsync(int id);
        Task CreateAsync(Team t);
        Task UpdateAsync(Team t);
        Task DeleteAsync(Team t);
        Task AddMemberAsync(int teamId, string userId);
        Task RemoveMemberAsync(int teamId, string userId);
        Task LinkProjectAsync(int teamId, int projectId);
        Task UnlinkProjectAsync(int teamId, int projectId);
    }
}
