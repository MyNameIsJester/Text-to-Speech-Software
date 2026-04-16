using AudioGuideAdmin.Models;
using AudioGuideAdmin.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AudioGuideAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var vm = new List<UserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                vm.Add(new UserListItemViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    RoleName = roles.FirstOrDefault() ?? "-"
                });
            }

            return View(vm.OrderBy(x => x.Email).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.RoleOptions = new List<string> { "Admin", "FoodStallOwner" };
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            ViewBag.RoleOptions = new List<string> { "Admin", "FoodStallOwner" };

            if (!ModelState.IsValid)
                return View(model);

            if (!await _roleManager.RoleExistsAsync(model.RoleName))
            {
                ModelState.AddModelError(nameof(model.RoleName), "Selected role does not exist.");
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}