using Ligeris.DataAcess.Dao;
using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Modelss;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.DataAcess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
		private DBKoneksi _db;
		public CompanyRepository(DBKoneksi db) :base(db)
		{
			_db = db;

		}
		public void Update(Company obj)
		{
			_db.Companies.Update(obj);
		}
	}
}
