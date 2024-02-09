using System.Linq.Expressions;

namespace Bookstore.DataAccess.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		//T - Category.
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
		T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);// Method to retriv a single entity of type T.
		void Add(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);
	}
}
