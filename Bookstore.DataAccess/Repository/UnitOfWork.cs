using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;

namespace Bookstore.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly Entities _entities;
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
		public ICompanyRepository Company { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }
		public IProductImageRepository ProductImage { get; private set; }

		public UnitOfWork(Entities entities)
        {
            _entities = entities;
			Category = new CategoryRepository(_entities);
			Product = new ProductRepository(_entities);
			Company = new CompanyRepository(_entities);
			ShoppingCart = new ShoppingCartRepository(_entities);
			ApplicationUser = new ApplicationUserRepository(_entities);
			OrderHeader = new OrderHeaderRepository(_entities);
			OrderDetail = new OrderDetailRepository(_entities);
			ProductImage = new ProductImageRepository(_entities);
        }

		public void Save()
		{
			_entities.SaveChanges();
		}
	}
}
