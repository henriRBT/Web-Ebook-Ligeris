using Ligeris.DataAcess.Dao;
using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Modelss;
using Ligeris.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Ligeris.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        //private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Data Category berhasil di Tambah";
                return RedirectToAction("Index");
            }
            /* if (_db.Categories.Any(c => c.Name == obj.Name))
             {
                 ModelState.AddModelError("Name", "Nama Category sudah digunakan");
             }*/
            return View();
        }

        //Ini untuk tampilan ke halaman Edit Category
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);
            //ada situasi yang dialami untuk melakukan beberapa perhitungan atau pemfilteran dan kemudian mendapatkan yang FirstOrDefault saja,
            //Category categoryFromDb2 = _db.Categories.FirstOrDefault(u => u.Id == id); //Ini digunakan untuk mengambil data yang bukan primary key jika data primary key tidak disarankan menggunakan ini
            //maka saya akan menggunakanpendekatan terakhir di sini.
            //Category categoryFromDb3 = _db.Categories.Where(u => u.Id == id).FirstOrDefault();
            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == obj.Id);
            if (categoryFromDb.Name == obj.Name)
            {
                if (categoryFromDb.DisplayOrder == obj.DisplayOrder)
                {
                    ModelState.AddModelError("Name", "Tidak ada perubahan yang dilakukan.");
                    ModelState.AddModelError("DisplayOrder", "Tidak ada perubahan yang dilakukan.");
                }
                else
                {
                    categoryFromDb.Name = obj.Name;
                    categoryFromDb.DisplayOrder = obj.DisplayOrder;
                    _unitOfWork.Save();
                    TempData["success"] = "Data Category berhasil di Update";
                    return RedirectToAction("Index");
                }
            }

            if (ModelState.IsValid)
            {
                categoryFromDb.Name = obj.Name;
                categoryFromDb.DisplayOrder = obj.DisplayOrder;
                _unitOfWork.Save();
                TempData["success"] = "Data Category berhasil di Update";
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category categoryFromDb = _unitOfWork.Category.Get(u => u.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Data Category berhasil di Delete";
            return RedirectToAction("Index");

        }
    }
}
