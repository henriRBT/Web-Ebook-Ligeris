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
	public class ShoopingRepository : Repository<ShoopingCard>, IShoopingRepository
    {
		private DBKoneksi _db;
		public ShoopingRepository(DBKoneksi db) :base(db)
		{
			_db = db;

		}
		public void Update(ShoopingCard obj)
		{
			_db.ShoopingCards.Update(obj);
		}
	}
}
