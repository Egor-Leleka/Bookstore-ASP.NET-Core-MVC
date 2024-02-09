using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookstoreWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class ShoppingCartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

		public ShoppingCartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(i => i.ApplicationUserId == userId,
				includeProperties: "Product"),
				OrderHeader = new()
			};

			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

			foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
			{
				shoppingCart.Product.ProductImages = productImages.Where(u=> u.ProductId == shoppingCart.Product.Id).ToList();
				shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
				ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
			}

			return View(ShoppingCartViewModel);
		}

		public IActionResult Plus(int cartId)
		{
			var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId);
			shoppingCartFromDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(shoppingCartFromDb);
			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId, tracked: true);

			if (shoppingCartFromDb.Count <= 1)
				
			_unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
			else
				shoppingCartFromDb.Count -= 1;
			HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == shoppingCartFromDb.ApplicationUserId).Count() - 1);
			_unitOfWork.ShoppingCart.Update(shoppingCartFromDb);


			_unitOfWork.Save();

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var shoppingCartFromDb = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId, tracked: true);
			HttpContext.Session.SetInt32(StaticDetails.SessionCart, _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == shoppingCartFromDb.ApplicationUserId).Count() - 1);
			_unitOfWork.ShoppingCart.Remove(shoppingCartFromDb);
			
			_unitOfWork.Save();
			
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel = new()
			{
				ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(i => i.ApplicationUserId == userId,
				includeProperties: "Product"),
				OrderHeader = new()
			};

			ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			ShoppingCartViewModel.OrderHeader.FirstName = ShoppingCartViewModel.OrderHeader.ApplicationUser.FirstName;
			ShoppingCartViewModel.OrderHeader.LastName = ShoppingCartViewModel.OrderHeader.ApplicationUser.LastName;
			ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
			ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
			ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
			ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

			foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
			{
				shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
				ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
			}

			return View(ShoppingCartViewModel);
		}


		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart
					.GetAll(i => i.ApplicationUserId == userId, includeProperties: "Product");

			ShoppingCartViewModel.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;

			var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			foreach (var shoppingCart in ShoppingCartViewModel.ShoppingCartList)
			{
				shoppingCart.Price = GetPriceBasedOnQuantity(shoppingCart);
				ShoppingCartViewModel.OrderHeader.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				// Customer.
				ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusPending;
				ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusPending;
			}
			else
			{
				// Company.
				ShoppingCartViewModel.OrderHeader.PaymentStatus = StaticDetails.PaymentStatusDelayedPayment;
				ShoppingCartViewModel.OrderHeader.OrderStatus = StaticDetails.StatusApproved;
			}

			_unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
			_unitOfWork.Save();

			foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};

				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

			if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				// Customer.
				var domain = "https://localhost:7224/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain
						+ $"customer/shoppingCart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
					CancelUrl = domain + "customer/shoppingCart/index",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment"
				};

				foreach (var item in ShoppingCartViewModel.ShoppingCartList)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmountDecimal = item.Price * 100,
							Currency = "nzd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Title,
							}
						},
						Quantity = item.Count
					};
					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				Session session = service.Create(options);
				_unitOfWork.OrderHeader.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();

				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
		}

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == id, includeProperties: "ApplicationUser");
			if (orderHeader.PaymentStatus != StaticDetails.PaymentStatusDelayedPayment)
			{
				// Customer order.
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					_unitOfWork.OrderHeader.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(
						id, StaticDetails.StatusApproved, StaticDetails.PaymentStatusApproved);
					_unitOfWork.Save();
				}
				HttpContext.Session.Clear();
			}

			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
				.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();

			return View(id);
		}

		private decimal GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
				return shoppingCart.Product.Price;
			else
			{
				if (shoppingCart.Count <= 100)
					return shoppingCart.Product.Price50;
				else
					return shoppingCart.Product.Price100;
			}
		}
	}
}
