using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repository
{
	public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
		private Entities _entities;

		public ProductImageRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(ProductImage category)
		{
			_entities.Update(category);
		}
	}
}
