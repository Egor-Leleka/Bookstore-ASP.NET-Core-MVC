using Bookstore.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Bookstore.Models.ViewModels
{
	public class RoleManagmentViewModel
	{
		public ApplicationUser ApplicationUser { get; set; }
		public IEnumerable<SelectListItem> RoleList { get; set; }
		public IEnumerable<SelectListItem> CompanyList { get; set; }
    }
}
