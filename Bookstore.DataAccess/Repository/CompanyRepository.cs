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
    internal class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private Entities _entities;

        public CompanyRepository(Entities entities) : base(entities)
        {
            _entities = entities;
        }

        public void Update(Company company)
        {
            _entities.Update(company);
        }
    }
}
