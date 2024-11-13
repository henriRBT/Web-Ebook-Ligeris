using Ligeris.Models;
using Microsoft.EntityFrameworkCore;

namespace Ligeris.DataAccess.Dao
{
    public class DBKoneksi : DbContext
    {
        public DBKoneksi(DbContextOptions<DBKoneksi> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Scifi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );
        }
    }
}
