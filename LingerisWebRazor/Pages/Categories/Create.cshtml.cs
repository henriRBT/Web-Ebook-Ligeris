using LingerisWebRazor.Dao;
using LingerisWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LingerisWebRazor.Pages.Categories
{
	[BindProperties] //Berfungsi untuk mengikat data atau mengambil data 
	public class CreateModel : PageModel
    {
		private readonly DBKoneksi _db;
		
		public Category Category { get; set; }
		public CreateModel(DBKoneksi db)
		{
			_db = db;
		}
		public void OnGet()
        {
        }

		public IActionResult OnPost()
		{
			if (_db.Categories.Any(c => c.Name == Category.Name))
			{
				ModelState.AddModelError("Category.Name", "Nama Category sudah digunakan");
			}

			else
			{
				_db.Categories.Add(Category);
				_db.SaveChanges();
				TempData["success"] = "Data Category berhasil di Tambah";
				return RedirectToPage("Index");
			}
			return Page();
		}
	}
}
