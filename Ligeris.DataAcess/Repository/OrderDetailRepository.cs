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
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
		private DBKoneksi _db;
		public OrderDetailRepository(DBKoneksi db) :base(db)
		{
			_db = db;

		}
		public void Update(OrderDetail obj)
		{
			_db.OrderDetails.Update(obj);
		}
	}
}
