using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagerMvc.Models;

namespace ProjectManagerMvc.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetAllAsync() =>
            await _userManager.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();

        public async Task<ApplicationUser?> GetByIdAsync(string id) =>
            await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return result;
            if (!string.IsNullOrWhiteSpace(role))
                await _userManager.AddToRoleAsync(user, role);
            return result;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user) =>
            await _userManager.UpdateAsync(user);

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user) =>
            await _userManager.DeleteAsync(user);

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user) =>
            await _userManager.GetRolesAsync(user);

        public async Task SetRoleAsync(ApplicationUser user, string role)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);
            if (!string.IsNullOrWhiteSpace(role))
                await _userManager.AddToRoleAsync(user, role);
        }
    }
}
