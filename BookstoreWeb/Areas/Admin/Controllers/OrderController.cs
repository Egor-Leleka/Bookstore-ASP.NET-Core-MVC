using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using Bookstore.Models.ViewModels;
using Bookstore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BookstoreWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderViewModel OrderViewModel { get; set; }

		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Details(int orderId)
		{
			OrderViewModel = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};
			return View(OrderViewModel);
		}

		[HttpPost, ActionName("Details")]
		public IActionResult Details_PAY_NOW()
		{
			OrderViewModel.OrderHeader = _unitOfWork.OrderHeader
				.Get(u => u.Id == OrderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
			OrderViewModel.OrderDetails = _unitOfWork.OrderDetail
				.GetAll(u => u.OrderHeaderId == OrderViewModel.OrderHeader.Id, includeProperties: "Product");

			// Customer.
			var domain = "https://localhost:7224/";
			var options = new SessionCreateOptions
			{
				SuccessUrl = domain
					+ $"admin/order/PaymentConfirmation?orderHeaderId={OrderViewModel.OrderHeader.Id}",
				CancelUrl = domain + $"admin/order/details?orderId={OrderViewModel.OrderHeader.Id}",
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment"
			};

			foreach (var item in OrderViewModel.OrderDetails)
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
			_unitOfWork.OrderHeader.UpdateStripePaymentId(OrderViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });

		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		public IActionResult UpdateOrderDetail()
		{
			var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderViewModel.OrderHeader.Id);
			orderHeaderFromDb.FirstName = OrderViewModel.OrderHeader.FirstName;
			orderHeaderFromDb.LastName = OrderViewModel.OrderHeader.LastName;
			orderHeaderFromDb.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
			orderHeaderFromDb.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
			orderHeaderFromDb.City = OrderViewModel.OrderHeader.City;
			orderHeaderFromDb.State = OrderViewModel.OrderHeader.State;
			orderHeaderFromDb.PostalCode = OrderViewModel.OrderHeader.PostalCode;

			if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.Carrier))
				orderHeaderFromDb.Carrier = OrderViewModel.OrderHeader.Carrier;

			if (!string.IsNullOrEmpty(OrderViewModel.OrderHeader.TrackingNumber))
				orderHeaderFromDb.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;

			_unitOfWork.OrderHeader.Update(orderHeaderFromDb);
			_unitOfWork.Save();
			TempData["success"] = "Order details updated seccessfully!";

			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(OrderViewModel.OrderHeader.Id, StaticDetails.StatusInProcess);
			_unitOfWork.Save();
			TempData["success"] = "Order details updated seccessfully!";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		public IActionResult ShipOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderViewModel.OrderHeader.Id);
			orderHeader.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
			orderHeader.Carrier = OrderViewModel.OrderHeader.Carrier;
			orderHeader.OrderStatus = StaticDetails.StatusShipped;
			orderHeader.ShippingDate = DateTime.Now;


			if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
				orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);

			_unitOfWork.OrderHeader.Update(orderHeader);
			_unitOfWork.Save();
			TempData["success"] = "Order shipped seccessfully!";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		public IActionResult CancelOrder()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderViewModel.OrderHeader.Id);

			if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusApproved)
			{
				var option = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(option);

				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled);
			}
			else
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, StaticDetails.StatusCancelled);

			}

			_unitOfWork.Save();
			TempData["success"] = "Order cancelled seccessfully!";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.OrderHeader.Id });
		}

		public IActionResult PaymentConfirmation(int orderHeaderId)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(o => o.Id == orderHeaderId);
			if (orderHeader.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment)
			{
				// Company order.
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					orderHeader.PaymentIntentId = session.PaymentIntentId;
					_unitOfWork.OrderHeader.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
					_unitOfWork.OrderHeader.UpdateStatus(
						orderHeaderId, orderHeader.OrderStatus, StaticDetails.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}
			return View(orderHeaderId);
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> orderHeadersList;

			if (User.IsInRole(StaticDetails.Role_Admin) || User.IsInRole(StaticDetails.Role_Employee))
			{
				orderHeadersList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
			}
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

				orderHeadersList = _unitOfWork.OrderHeader
					.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
			}

			switch (status)
			{
				case "pending":
					orderHeadersList = orderHeadersList.Where(u => u.PaymentStatus == StaticDetails.PaymentStatusDelayedPayment);
					break;
				case "inprocess":
					orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == StaticDetails.StatusInProcess);
					break;
				case "completed":
					orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == StaticDetails.StatusShipped);
					break;
				case "approved":
					orderHeadersList = orderHeadersList.Where(u => u.OrderStatus == StaticDetails.StatusApproved);
					break;
				default:
					break;
			}

			return Json(new { data = orderHeadersList });
		}


		#endregion
	}
}
