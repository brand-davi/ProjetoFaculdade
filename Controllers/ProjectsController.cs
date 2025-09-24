using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagerMvc.Models;
using ProjectManagerMvc.Services;

namespace ProjectManagerMvc.Controllers
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class ProjectsController : Controller
    {
        private readonly IProjectService _projects;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(IProjectService projects, UserManager<ApplicationUser> userManager)
        {
            _projects = projects;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _projects.GetAllAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Managers = await GetManagersSelectListAsync();
            return View(new Project());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Project p)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Managers = await GetManagersSelectListAsync();
                return View(p);
            }
            await _projects.CreateAsync(p);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _projects.GetByIdAsync(id);
            if (p == null) return NotFound();
            ViewBag.Managers = await GetManagersSelectListAsync();
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Project p)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Managers = await GetManagersSelectListAsync();
                return View(p);
            }
            await _projects.UpdateAsync(p);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _projects.GetByIdAsync(id);
            if (p == null) return NotFound();
            await _projects.DeleteAsync(p);
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetManagersSelectListAsync()
        {
            var users = _userManager.Users.ToList();
            var managers = new List<SelectListItem>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                if (roles.Contains("Manager") || roles.Contains("Admin"))
                    managers.Add(new SelectListItem { Value = u.Id, Text = $"{u.FullName} ({u.Email})" });
            }
            return managers;
        }
    }
}
