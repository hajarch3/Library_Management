using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Gestion_bibliot.Models;

namespace Gestion_bibliot.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public AdminController()
        {
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set { _userManager = value; }
        }

        public RoleManager<IdentityRole> RoleManager
        {
            get
            {
                return _roleManager ?? new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(HttpContext.GetOwinContext().Get<ApplicationDbContext>()));
            }
            private set { _roleManager = value; }
        }

        // GET: /Admin
        public ActionResult Index()
        {
            var users = UserManager.Users.ToList().Select(u => new AdminEditViewModel
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role,
                IsActive = u.IsActive
            }).ToList();

            return View(users);
        }

        // GET: /Admin/Edit/{id}
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(400);

            var user = UserManager.FindById(id);
            if (user == null)
                return HttpNotFound();

            var model = new AdminEditViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive,
                AvailableRoles = RoleManager.Roles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        // POST: /Admin/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = UserManager.FindById(model.Id);
            if (user == null)
                return HttpNotFound();

            // Update basic properties
            user.IsActive = model.IsActive;
            user.FullName = model.FullName ?? user.FullName;

            // Update roles: remove any role not equal to the selected one
            var currentRoles = UserManager.GetRoles(user.Id).ToList();
            foreach (var r in currentRoles)
            {
                if (r != model.Role)
                {
                    UserManager.RemoveFromRole(user.Id, r);
                }
            }

            if (!string.IsNullOrEmpty(model.Role) && !currentRoles.Contains(model.Role))
            {
                if (!RoleManager.RoleExists(model.Role))
                {
                    RoleManager.Create(new IdentityRole(model.Role));
                }
                UserManager.AddToRole(user.Id, model.Role);
                user.Role = model.Role; // keep custom Role property in sync
            }

            var result = UserManager.Update(user);
            if (!result.Succeeded)
            {
                foreach (var err in result.Errors)
                    ModelState.AddModelError("", err);

                model.AvailableRoles = RoleManager.Roles.Select(r => r.Name).ToList();
                return View(model);
            }

            return RedirectToAction("Index");
        }
    }
}
