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
	public class ProductRepository : Repository<Product>, IProductRepository
	{
		private DBKoneksi _db;
		public ProductRepository(DBKoneksi db) :base(db)
		{
			_db = db;

		}
		public void Update(Product obj)
		{
			var objFromDb = _db.Products.FirstOrDefault(p => p.Id == obj.Id);
			if (objFromDb != null)
			{
                objFromDb.Title = obj.Title;
				objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
				objFromDb.Price100 = obj.Price100;
				objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price500 = obj.Price500;
                objFromDb.Deskripsi = obj.Deskripsi;
                objFromDb.CategoryId = obj.CategoryId;
				objFromDb.Author = obj.Author;
                objFromDb.ProductImages = obj.ProductImages;
            }

        }
	}
}
