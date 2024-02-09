using Azure;
using Bookstore.DataAccess.Data;
using Bookstore.DataAccess.Repository.IRepository;
using Bookstore.Models.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Bookstore.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private Entities _entities;
		private DbSet<T> entitiesSet;

		public Repository(Entities entities)
		{
			_entities = entities;
			this.entitiesSet = _entities.Set<T>();

			// Loading related data for the Products entity (when fetching each product from Products,
			// the related Category data (ForeignKey) is also automatically loaded).
			_entities.Products.Include(c => c.Category).Include(c => c.CategoryId);
		}

		public void Add(T entity)
		{
			entitiesSet.Add(entity);
		}

		public void Remove(T entity)
		{
			entitiesSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entities)
		{
			entitiesSet.RemoveRange(entities);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query;

			if (tracked)
				query = entitiesSet;
			else
				query = entitiesSet.AsNoTracking();

			query = query.Where(filter);

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProperty in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}
			return query.FirstOrDefault();
		}



		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
		{
			IQueryable<T> query = entitiesSet;

			if(filter != null)
				query = query.Where(filter);

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProperty in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}
			return query.ToList();
		}

		
	}
}
