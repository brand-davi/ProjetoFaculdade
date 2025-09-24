using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagerMvc.Models;
using ProjectManagerMvc.Services;
using ProjectManagerMvc.ViewModels;

namespace ProjectManagerMvc.Controllers
{
    [Authorize(Policy = "RequireAdmin")]
    public class UsersController : Controller
    {
        private readonly IUserService _users;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserService users, UserManager<ApplicationUser> userManager)
        {
            _users = users;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _users.GetAllAsync();
            var vms = new List<UserListItemViewModel>();
            foreach (var u in list)
            {
                var roles = await _users.GetRolesAsync(u);
                vms.Add(new UserListItemViewModel
                {
                    Id = u.Id, FullName = u.FullName, Email = u.Email ?? "", CPF = u.CPF,
                    Position = u.Position, Role = roles.FirstOrDefault() ?? "(sem perfil)"
                });
            }
            return View(vms);
        }

        [HttpGet]
        public IActionResult Create() => View(new CreateUserViewModel());

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = new ApplicationUser
            {
                FullName = vm.FullName,
                CPF = vm.CPF,
                Email = vm.Email,
                UserName = vm.Email,
                Position = vm.Position
            };

            var result = await _users.CreateAsync(user, vm.Password, vm.Role);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", $"{e.Code}: {e.Description}");
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var u = await _users.GetByIdAsync(id);
            if (u == null) return NotFound();
            var roles = await _users.GetRolesAsync(u);
            var vm = new EditUserViewModel
            {
                Id = u.Id, FullName = u.FullName, CPF = u.CPF, Email = u.Email ?? "",
                Position = u.Position, Role = roles.FirstOrDefault() ?? "Collaborator"
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var u = await _users.GetByIdAsync(vm.Id);
            if (u == null) return NotFound();

            u.FullName = vm.FullName;
            u.CPF = vm.CPF;
            u.Email = vm.Email;
            u.UserName = vm.Email;
            u.Position = vm.Position;

            var result = await _users.UpdateAsync(u);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", $"{e.Code}: {e.Description}");
                return View(vm);
            }

            await _users.SetRoleAsync(u, vm.Role);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var u = await _users.GetByIdAsync(id);
            if (u == null) return NotFound();
            var result = await _users.DeleteAsync(u);
            if (!result.Succeeded) return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));
            return RedirectToAction(nameof(Index));
        }
    }
}
