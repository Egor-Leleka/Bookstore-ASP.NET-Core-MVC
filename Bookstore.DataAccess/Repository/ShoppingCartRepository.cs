using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.DataAccess.Repository
{
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private Entities _entities;

		public ShoppingCartRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(ShoppingCart shoppingCart)
		{
			_entities.ShoppingCarts.Update(shoppingCart);
		}
	}
}
