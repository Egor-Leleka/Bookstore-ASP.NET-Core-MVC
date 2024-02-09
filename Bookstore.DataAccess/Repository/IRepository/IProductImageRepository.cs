using Bookstore.Models.Models;

namespace Bookstore.DataAccess.Repository.IRepository
{
	public interface IProductImageRepository : IRepository<ProductImage>
	{
		void Update(ProductImage category);
	}
}
