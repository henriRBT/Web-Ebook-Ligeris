using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Ligeris.DataAcess.Dao;
using Ligeris.DataAcess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ligeris.DataAcess.Repository
{
	public class Repository<T>: IRepository<T> where T : class
	{
		private readonly DBKoneksi _db;
		internal DbSet<T> dbSet;	
		public Repository(DBKoneksi db)
		{
			_db = db;
			this.dbSet = _db.Set<T>();
			_db.Products.Include(u => u.Category).Include(u => u.CategoryId);
		}
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includePropertis = null , bool tracked = false)
		{
			IQueryable<T> query;

            if (tracked)
			{
                query = dbSet;
            }
			else
			{
               query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includePropertis))
            {
                foreach (var includeProp in includePropertis
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }
		//Category, CoverType
		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includePropertis = null)
        {
			IQueryable<T> query = dbSet;
			if (filter != null)
			{
				query = query.Where(filter);
			}
			if (!string.IsNullOrEmpty(includePropertis))
			{
				foreach (var includeProp in includePropertis
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
			return query.ToList();
		}

		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}
	}
}
