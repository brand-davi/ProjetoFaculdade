using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Data;
using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDbContext _db;
        public ProjectService(ApplicationDbContext db) { _db = db; }

        public async Task<List<Project>> GetAllAsync() =>
            await _db.Projects.Include(p => p.Manager).AsNoTracking().OrderBy(p => p.Name).ToListAsync();

        public async Task<Project?> GetByIdAsync(int id) =>
            await _db.Projects.Include(p => p.Manager)
                              .Include(p => p.Tasks)
                              .FirstOrDefaultAsync(p => p.Id == id);

        public async Task CreateAsync(Project p)
        {
            _db.Projects.Add(p);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project p)
        {
            _db.Projects.Update(p);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Project p)
        {
            _db.Projects.Remove(p);
            await _db.SaveChangesAsync();
        }
    }
}
