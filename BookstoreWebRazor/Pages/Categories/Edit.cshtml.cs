using BookstoreWebRazor.Data;
using BookstoreWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreWebRazor.Pages.Categories
{
	public class EditModel : PageModel
	{
		private readonly Entities _entities;

		[BindProperty]
		public Category? Category { get; set; }

		public EditModel(Entities entities)
		{
			_entities = entities;
		}

		public void OnGet(int? id) 
		{
			if(id != null && id != 0)
				Category = _entities.Categories.FirstOrDefault(c => c.Id == id);

		}

		public IActionResult OnPost()
		{
			if (ModelState.IsValid)
			{
				_entities.Categories.Update(Category);
				_entities.SaveChanges();
				TempData["success"] = "Category updated successfully!";
				return RedirectToPage("Index");
			}

			return Page();
		}
	}
}
