using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repository
{
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
	{
		private Entities _entities;

		public OrderDetailRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(OrderDetail orderDetails)
		{
			_entities.Update(orderDetails);
		}
	}
}
