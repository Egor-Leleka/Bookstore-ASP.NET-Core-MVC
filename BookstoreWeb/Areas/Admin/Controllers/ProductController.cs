using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookstoreWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = StaticDetails.Role_Admin)]
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

			return View(productList);
		}

		[HttpGet]
		public IActionResult Upsert(int? id)
		{
			ProductViewModel productViewModel = new()
			{
				CategoryList = _unitOfWork.Category
				.GetAll().Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString()
				}),
				Product = new Product()
			};

			if (id == null || id == 0)
			{
				// Create.
				return View(productViewModel);
			}
			else
			{
				// Update.
				productViewModel.Product = _unitOfWork.Product.Get(p => p.Id == id, includeProperties: "ProductImages");
				return View(productViewModel);
			}
		}

		[HttpPost]
		public IActionResult Upsert(ProductViewModel productViewModel, List<IFormFile>? files)
		{
			if (ModelState.IsValid)
			{
				if (productViewModel.Product.Id == 0)
				{
					_unitOfWork.Product.Add(productViewModel.Product);
				}
				else
				{
					_unitOfWork.Product.Update(productViewModel.Product);
				}

				_unitOfWork.Save();

				string wwwRootPath = _webHostEnvironment.WebRootPath;

				foreach (IFormFile file in files)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = @"images\products\product-" + productViewModel.Product.Id;
					string finalPath = Path.Combine(wwwRootPath, productPath);

					if (!Directory.Exists(finalPath))
						Directory.CreateDirectory(finalPath);

					using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}

					ProductImage productImage = new()
					{
						ImageUrl = @"\" + productPath + @"\" + fileName,
						ProductId = productViewModel.Product.Id,
					};

					if (productViewModel.Product.ProductImages == null)
						productViewModel.Product.ProductImages = new List<ProductImage>();

					productViewModel.Product.ProductImages.Add(productImage);
				}

				_unitOfWork.Product.Update(productViewModel.Product);
				_unitOfWork.Save();

				TempData["success"] = "Product created/updated seccessfully!";
				return RedirectToAction("Index", "Product");
			}
			else
			{
				productViewModel.CategoryList = _unitOfWork.Category
				.GetAll().Select(c => new SelectListItem
				{
					Text = c.Name,
					Value = c.Id.ToString()
				});
				return View(productViewModel);
			}
		}

		[HttpGet]
		public IActionResult DeleteImage(int imageId)
		{
			var imageToBeDeleted = _unitOfWork.ProductImage.Get(i => i.Id == imageId);
			int productId = imageToBeDeleted.ProductId;
			if (imageToBeDeleted != null)
			{
				if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
				{
					var oldImagePath =
									Path.Combine(_webHostEnvironment.WebRootPath,
									imageToBeDeleted.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(oldImagePath))
						System.IO.File.Delete(oldImagePath);

				}

				_unitOfWork.ProductImage.Remove(imageToBeDeleted);
				_unitOfWork.Save();
				TempData["success"] = "Image deleted seccessfully!";
			}

			return RedirectToAction(nameof(Upsert), new { id = productId });
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

			return Json(new { data = productList });
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(p => p.Id == id);
			if (productToBeDeleted == null)
				return Json(new { success = false, message = "Error occured while deleting" });

			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string productPath = @"images\products\product-" + id;
			string finalPath = Path.Combine(wwwRootPath, productPath);

			if (Directory.Exists(finalPath))
			{
				string[] filePaths = Directory.GetFiles(finalPath);
				foreach (string filePath in filePaths)
				{
					System.IO.File.Delete(filePath);
				}
				Directory.Delete(finalPath);
			}

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Product deleted successfully" });
		}


		#endregion
	}
}
