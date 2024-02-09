using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookstoreWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RoleManagment(string userId)
        {
            RoleManagmentViewModel roleVM = new RoleManagmentViewModel()
            {
				ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            roleVM.ApplicationUser.Role = _userManager
                .GetRolesAsync(_unitOfWork.ApplicationUser.Get(u=>u.Id == userId))
                .GetAwaiter().GetResult().FirstOrDefault();

            return View(roleVM);
        }

        [HttpPost]
        public IActionResult RoleManagment(RoleManagmentViewModel roleVm)
        {
            string oldRole = _userManager
				.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleVm.ApplicationUser.Id))
				.GetAwaiter().GetResult().FirstOrDefault();

			ApplicationUser applicationUser = _unitOfWork.ApplicationUser
					.Get(u => u.Id == roleVm.ApplicationUser.Id);

			if (!(roleVm.ApplicationUser.Role == oldRole))
            {
                if (roleVm.ApplicationUser.Role == StaticDetails.Role_Company)
                    applicationUser.CompanyId = roleVm.ApplicationUser.CompanyId;

                if (oldRole == StaticDetails.Role_Company)
                    applicationUser.CompanyId = null;

				_unitOfWork.ApplicationUser.Update(applicationUser);
				_unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleVm.ApplicationUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if(oldRole == StaticDetails.Role_Company && applicationUser.CompanyId != roleVm.ApplicationUser.CompanyId)
                {
                    applicationUser.CompanyId=roleVm.ApplicationUser.CompanyId;
                    _unitOfWork.ApplicationUser.Update(applicationUser);
                    _unitOfWork.Save();
                }
            }

            return RedirectToAction("Index");
        }



        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();
            
            foreach (var user in userList)
            {
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new Company { Name = "" };
                }
            }

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var userFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            if (userFromDb == null)
                return Json(new { sucsess = false, message = "Error while Locking/Unlicking user" });

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
				userFromDb.LockoutEnd = DateTime.Now; // Unlock locked user.
            else
				userFromDb.LockoutEnd = DateTime.Now.AddYears(100); // Lock user for 100 years.
            
            _unitOfWork.ApplicationUser.Update(userFromDb);
            _unitOfWork.Save();

            return Json(new { success = true, message = "User Locked/UnLocked successfully" });
        }

        #endregion
    }
}
