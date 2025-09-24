using ProjectManagerMvc.Models;
using Microsoft.AspNetCore.Identity;

namespace ProjectManagerMvc.Services
{
    public interface IUserService
    {
        Task<List<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password, string role);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task SetRoleAsync(ApplicationUser user, string role);
    }
}
