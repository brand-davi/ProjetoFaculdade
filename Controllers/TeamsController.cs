using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagerMvc.Models;
using ProjectManagerMvc.Services;

namespace ProjectManagerMvc.Controllers
{
    [Authorize(Policy = "RequireManagerOrAdmin")]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teams;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IProjectService _projects;

        public TeamsController(ITeamService teams, UserManager<ApplicationUser> userManager, IProjectService projects)
        {
            _teams = teams;
            _userManager = userManager;
            _projects = projects;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _teams.GetAllAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Team());

        [HttpPost]
        public async Task<IActionResult> Create(Team t)
        {
            if (!ModelState.IsValid) return View(t);
            await _teams.CreateAsync(t);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var t = await _teams.GetByIdAsync(id);
            if (t == null) return NotFound();

            ViewBag.AllUsers = _userManager.Users
                .Select(u => new SelectListItem { Value = u.Id, Text = $"{u.FullName} ({u.Email})" })
                .ToList();

            var projs = await _projects.GetAllAsync();
            ViewBag.AllProjects = projs.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();

            return View(t);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Team t)
        {
            if (!ModelState.IsValid) return View(t);
            await _teams.UpdateAsync(t);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _teams.GetByIdAsync(id);
            if (t == null) return NotFound();
            await _teams.DeleteAsync(t);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddMember(int teamId, string userId)
        {
            await _teams.AddMemberAsync(teamId, userId);
            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMember(int teamId, string userId)
        {
            await _teams.RemoveMemberAsync(teamId, userId);
            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        [HttpPost]
        public async Task<IActionResult> LinkProject(int teamId, int projectId)
        {
            await _teams.LinkProjectAsync(teamId, projectId);
            return RedirectToAction(nameof(Details), new { id = teamId });
        }

        [HttpPost]
        public async Task<IActionResult> UnlinkProject(int teamId, int projectId)
        {
            await _teams.UnlinkProjectAsync(teamId, projectId);
            return RedirectToAction(nameof(Details), new { id = teamId });
        }
    }
}
