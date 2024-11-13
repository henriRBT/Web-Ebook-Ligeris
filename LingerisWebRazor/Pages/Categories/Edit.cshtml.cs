using LingerisWebRazor.Dao;
using LingerisWebRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LingerisWebRazor.Pages.Categories
{
	[BindProperties]
    public class EditModel : PageModel
    {
		private readonly DBKoneksi _db;
		public Category Category { get; set; }
		public EditModel(DBKoneksi db)
		{
			_db = db;
		}
		public void OnGet(int? id)
        {
			if (id != null && id != 0)
			{
				Category = _db.Categories.Find(id);
			}
		}

		public IActionResult OnPost()
		{
			var categoryFromDb = _db.Categories.Find(Category.Id);
			if (categoryFromDb.Name == Category.Name)
			{
				if (categoryFromDb.DisplayOrder == Category.DisplayOrder)
				{
					ModelState.AddModelError("Category.Name", "Tidak ada perubahan yang dilakukan.");
					ModelState.AddModelError("Category.DisplayOrder", "Tidak ada perubahan yang dilakukan.");
				}
				else
				{
					categoryFromDb.Name = Category.Name;
					categoryFromDb.DisplayOrder = Category.DisplayOrder;
					_db.SaveChanges();
					TempData["success"] = "Data Category berhasil di Update";
					return RedirectToPage("Index");
				}
			}

			if (ModelState.IsValid)
			{
				categoryFromDb.Name = Category.Name;
				categoryFromDb.DisplayOrder = Category.DisplayOrder;
				_db.SaveChanges();
				TempData["success"] = "Data Category berhasil di Update";
				return RedirectToPage("Index");
			}
			return Page();
		}
	}
}
