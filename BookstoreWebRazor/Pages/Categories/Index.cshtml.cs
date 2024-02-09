using BookstoreWebRazor.Data;
using BookstoreWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookstoreWebRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly Entities _entities;
        public List<Category> Categories { get; set; }

        public IndexModel(Entities entites)
        {
            _entities = entites;
        }

        public void OnGet()
        {
            Categories = _entities.Categories.ToList();
        }
    }
}
