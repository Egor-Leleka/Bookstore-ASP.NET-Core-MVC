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
	internal class ProductRepository : Repository<Product>, IProductRepository
	{
		private Entities _entities;

		public ProductRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(Product product)
		{
			var productFromDb = _entities.Products.FirstOrDefault(p => p.Id == product.Id);
			if (productFromDb != null)
			{
				productFromDb.Title = product.Title;
				productFromDb.ISBN = product.ISBN;
				productFromDb.ListPrice = product.ListPrice;
				productFromDb.Price = product.Price;
				productFromDb.Price50 = product.Price50;
				productFromDb.Price100 = product.Price100;
				productFromDb.Author = product.Author;
				productFromDb.Description = product.Description;
				productFromDb.Category = product.Category;
				productFromDb.CategoryId = product.CategoryId;
				productFromDb.ProductImages = product.ProductImages;
			}
		}
	}
}
