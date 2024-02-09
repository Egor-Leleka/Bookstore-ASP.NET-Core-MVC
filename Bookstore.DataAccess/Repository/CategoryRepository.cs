using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repository
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private Entities _entities;

		public CategoryRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(Category category)
		{
			_entities.Update(category);
		}
	}
}
