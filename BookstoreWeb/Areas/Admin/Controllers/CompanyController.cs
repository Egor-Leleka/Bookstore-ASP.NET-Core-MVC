using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Role_Admin)]
	public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public IActionResult Index()
		{
			List<Company> companyList = _unitOfWork.Company.GetAll().ToList();

			return View(companyList);
		}

		[HttpGet]
		public IActionResult Upsert(int? id)
		{
			if (id == null || id == 0)
			{
				// Create.
				return View(new Company());
			}
			else
			{
				// Update.
				var company = _unitOfWork.Company.Get(c => c.Id == id);
				return View(company);
			}
		}

		[HttpPost]
		public IActionResult Upsert(Company company)
		{
			if (ModelState.IsValid)
			{
				if (company.Id == 0)
					_unitOfWork.Company.Add(company);
				else
					_unitOfWork.Company.Update(company);


				_unitOfWork.Save();
				TempData["success"] = "Product created seccessfully!";
				return RedirectToAction("Index", "Company");
			}
			else
			{
				return View(company);
			}


		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> companyList = _unitOfWork.Company.GetAll().ToList();

			return Json(new { data = companyList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var companyToBeDeleted = _unitOfWork.Company.Get(p => p.Id == id);
			if (companyToBeDeleted == null)
				return Json(new { success = false, message = "Error occured while deleting" });


			_unitOfWork.Company.Remove(companyToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Company deleted successfully" });
		}


		#endregion


	}
}
