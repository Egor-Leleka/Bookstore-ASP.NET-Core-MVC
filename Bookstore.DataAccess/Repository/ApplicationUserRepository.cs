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
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
	{
		private Entities _entities;
		public ApplicationUserRepository(Entities entities) : base(entities)
		{
			_entities = entities;
		}

		public void Update(ApplicationUser applicationUser)
		{
			_entities.ApplicationUsers.Update(applicationUser);
		}
	}
}
