using BookstoreWebRazor.Data;
using BookstoreWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreWebRazor.Pages.Categories
{
	public class DeleteModel : PageModel
	{
		private readonly Entities _entities;

		[BindProperty]
		public Category? Category { get; set; }

		public DeleteModel(Entities entities)
		{
			_entities = entities;
		}

		public void OnGet(int? id)
		{
			if (id != null && id != 0)
				Category = _entities.Categories.FirstOrDefault(c => c.Id == id);
		}

		public IActionResult OnPost()
		{
			Category? category = _entities.Categories.Find(Category.Id);
			if (category == null)
				return NotFound();

			_entities.Categories.Remove(category);
			_entities.SaveChanges();
			TempData["success"] = "Category deleted successfully!";
			return RedirectToPage("Index");
		}
	}
}
