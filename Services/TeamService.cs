using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Data;
using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _db;
        public TeamService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Team>> GetAllAsync() =>
            await _db.Teams.AsNoTracking().OrderBy(t => t.Name).ToListAsync();

        public async Task<Team?> GetByIdAsync(int id) =>
            await _db.Teams
                .Include(t => t.TeamUsers).ThenInclude(tu => tu.User)
                .Include(t => t.ProjectTeams).ThenInclude(pt => pt.Project)
                .FirstOrDefaultAsync(t => t.Id == id);

        public async Task CreateAsync(Team t)
        {
            _db.Teams.Add(t);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Team t)
        {
            _db.Teams.Update(t);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Team t)
        {
            _db.Teams.Remove(t);
            await _db.SaveChangesAsync();
        }

        public async Task AddMemberAsync(int teamId, string userId)
        {
            if (!await _db.TeamUsers.AnyAsync(x => x.TeamId == teamId && x.UserId == userId))
                _db.TeamUsers.Add(new TeamUser { TeamId = teamId, UserId = userId });
            await _db.SaveChangesAsync();
        }

        public async Task RemoveMemberAsync(int teamId, string userId)
        {
            var link = await _db.TeamUsers.FirstOrDefaultAsync(x => x.TeamId == teamId && x.UserId == userId);
            if (link != null)
            {
                _db.TeamUsers.Remove(link);
                await _db.SaveChangesAsync();
            }
        }

        public async Task LinkProjectAsync(int teamId, int projectId)
        {
            if (!await _db.ProjectTeams.AnyAsync(x => x.TeamId == teamId && x.ProjectId == projectId))
                _db.ProjectTeams.Add(new ProjectTeam { TeamId = teamId, ProjectId = projectId });
            await _db.SaveChangesAsync();
        }

        public async Task UnlinkProjectAsync(int teamId, int projectId)
        {
            var link = await _db.ProjectTeams.FirstOrDefaultAsync(x => x.TeamId == teamId && x.ProjectId == projectId);
            if (link != null)
            {
                _db.ProjectTeams.Remove(link);
                await _db.SaveChangesAsync();
            }
        }
    }
}
