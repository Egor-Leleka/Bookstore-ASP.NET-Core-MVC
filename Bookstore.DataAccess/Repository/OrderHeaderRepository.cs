using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repository
{
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private Entities _entities;

		public OrderHeaderRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(OrderHeader orderHeader)
		{
			_entities.Update(orderHeader);
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _entities.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
				if (!string.IsNullOrEmpty(paymentStatus))
					orderFromDb.PaymentStatus = paymentStatus;
			}
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _entities.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (!string.IsNullOrEmpty(sessionId))
				orderFromDb.SessionId = sessionId;

			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.SessionId = paymentIntentId;
				orderFromDb.PaymentDate = DateTime.Now;
			}

		}
	}
}
