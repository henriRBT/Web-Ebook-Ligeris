using LingerisWebRazor.Dao;
using LingerisWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LingerisWebRazor.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly DBKoneksi _db;
        public List<Category> CategoryList { get; set; }
        public IndexModel(DBKoneksi db)
        {
            _db = db;
        }
        public void OnGet()
        {
            CategoryList = _db.Categories.ToList();
        }
    }
}
