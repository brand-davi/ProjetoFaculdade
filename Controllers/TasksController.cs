using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagerMvc.Models;
using ProjectManagerMvc.Services;

namespace ProjectManagerMvc.Controllers
{
    public class TasksController : Controller
    {
        private readonly ITaskService _tasks;
        private readonly IProjectService _projects;
        private readonly UserManager<ApplicationUser> _userManager;

        public TasksController(ITaskService tasks, IProjectService projects, UserManager<ApplicationUser> userManager)
        {
            _tasks = tasks;
            _projects = projects;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var list = await _tasks.GetAllAsync();
            return View(list);
        }

        [Authorize(Policy = "RequireManagerOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var projs = await _projects.GetAllAsync();
            ViewBag.Projects = projs.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
            ViewBag.Users = _userManager.Users.Select(u => new SelectListItem { Value = u.Id, Text = $"{u.FullName} ({u.Email})" }).ToList();
            return View(new TaskItem());
        }

        [Authorize(Policy = "RequireManagerOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(TaskItem t)
        {
            if (!ModelState.IsValid)
            {
                var projs = await _projects.GetAllAsync();
                ViewBag.Projects = projs.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
                ViewBag.Users = _userManager.Users.Select(u => new SelectListItem { Value = u.Id, Text = $"{u.FullName} ({u.Email})" }).ToList();
                return View(t);
            }
            await _tasks.CreateAsync(t);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "RequireManagerOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var t = await _tasks.GetByIdAsync(id);
            if (t == null) return NotFound();
            var projs = await _projects.GetAllAsync();
            ViewBag.Projects = projs.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name }).ToList();
            ViewBag.Users = _userManager.Users.Select(u => new SelectListItem { Value = u.Id, Text = $"{u.FullName} ({u.Email})" }).ToList();
            return View(t);
        }

        [Authorize(Policy = "RequireManagerOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Edit(TaskItem t)
        {
            if (!ModelState.IsValid) return View(t);
            await _tasks.UpdateAsync(t);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "RequireManagerOrAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _tasks.GetByIdAsync(id);
            if (t == null) return NotFound();
            await _tasks.DeleteAsync(t);
            return RedirectToAction(nameof(Index));
        }

        // Collaborator: can only update OWN task status
        [Authorize(Roles = "Collaborator,Manager,Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateMyStatus(int id, ProjectManagerMvc.Models.TaskStatus status)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();
            try
            {
                await _tasks.UpdateOwnStatusAsync(id, userId, status);
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
