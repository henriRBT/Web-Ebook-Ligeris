using Ligeris.DataAcess.Dao;
using Ligeris.DataAcess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ligeris.DataAcess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private DBKoneksi _db;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoopingRepository Shooping { get; private set; }
        public IAplikasiUserRepository AplikasiUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public UnitOfWork(DBKoneksi db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            Shooping = new ShoopingRepository(_db);
            AplikasiUser = new AplikasiUserRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            ProductImage = new ProductImageRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
