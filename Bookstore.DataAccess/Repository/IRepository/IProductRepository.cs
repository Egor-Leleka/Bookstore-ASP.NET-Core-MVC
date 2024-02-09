using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repository.IRepository
{
	public interface IProductRepository : IRepository<Product>
	{
		void Update(Product product);
	}
}
