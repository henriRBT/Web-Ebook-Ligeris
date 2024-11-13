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
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private DBKoneksi _db;
		public OrderHeaderRepository(DBKoneksi db) :base(db)
		{
			_db = db;

		}
		public void Update(OrderHeader obj)
		{
			_db.OrderHeaders.Update(obj);
		}
		public void UpdateStatus(int id, string StatusOrder, string? statusPembayaran = null)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u =>u.Id == id);
			if (orderFromDb !=null)
			{
				orderFromDb.StatusOrder = StatusOrder;
				if (!string.IsNullOrEmpty(statusPembayaran))
				{
					orderFromDb.StatusPembayaran = statusPembayaran;
				}
			}
		}

		public void UpdateStripePaymenID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (!string.IsNullOrEmpty(sessionId))
			{
				orderFromDb.SessionId = sessionId;
			}
			if (!string.IsNullOrEmpty(paymentIntentId))
			{
				orderFromDb.PaymentIntentId = paymentIntentId;
				orderFromDb.TanggalPembayaran = DateTime.Now;
			}
		}
	}
}
