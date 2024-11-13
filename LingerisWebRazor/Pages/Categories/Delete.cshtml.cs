using LingerisWebRazor.Dao;
using LingerisWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LingerisWebRazor.Pages.Categories
{
	[BindProperties]
    public class DeleteModel : PageModel
    {
		private readonly DBKoneksi _db;

		public Category Category { get; set; }
		public DeleteModel(DBKoneksi db)
		{
			_db = db;
		}
		public void OnGet(int id)
		{
			if (id != null && id != 0)
			{
				Category = _db.Categories.Find(id);
			}
		}

		public IActionResult OnPost()
		{
			Category? obj = _db.Categories.Find(Category.Id);
			if (obj == null)
			{
				return NotFound();
			}
			_db.Categories.Remove(obj);
			_db.SaveChanges();
			TempData["success"] = "Data Category berhasil di Delete";
			return RedirectToPage("Index");
		}
		
	}
}
