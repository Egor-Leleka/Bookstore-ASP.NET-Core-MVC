using BookstoreWebRazor.Data;
using BookstoreWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreWebRazor.Pages.Categories
{
	public class CreateModel : PageModel
	{
		private readonly Entities _entities;

		[BindProperty]
		public Category? Category { get; set; }

		public CreateModel(Entities entities)
		{
			_entities = entities;
		}

		public void OnGet() { }

		public IActionResult OnPost()
		{
			if(ModelState.IsValid)
			{
				_entities.Categories.Add(Category);
				_entities.SaveChanges();
				TempData["success"] = "Category crteated successfully!";
				return RedirectToPage("Index");
			}
			
			return Page();
		}
	}
}
