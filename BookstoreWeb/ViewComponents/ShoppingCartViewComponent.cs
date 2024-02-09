using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookstoreWeb.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				if(HttpContext.Session.GetInt32(StaticDetails.SessionCart) == null)
				{
					HttpContext.Session.SetInt32(StaticDetails.SessionCart, 
						_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
				}
					
				return View(HttpContext.Session.GetInt32(StaticDetails.SessionCart));
			}
			else
			{
				HttpContext.Session.Clear();

				return View(0);
			}
		}
	}
}
