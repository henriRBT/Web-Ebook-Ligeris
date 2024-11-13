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
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private DBKoneksi _db;
		public CategoryRepository(DBKoneksi db) :base(db)
		{
			_db = db;

		}
		public void Update(Category obj)
		{
			_db.Categories.Update(obj);
		}
	}
}
