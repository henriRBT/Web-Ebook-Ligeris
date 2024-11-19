using Ligeris.DataAcess.Dao;
using Ligeris.DataAcess.Repository;
using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Modelss;
using Ligeris.Modelss.ViewModels;
using Ligeris.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace Ligeris.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfwork; 
        public UserController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfwork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
		}
        public IActionResult Index()
        {
			return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            RoleManagemenVM RoleVM = new RoleManagemenVM() {
                //mengambil data user berdasarkan userID setelah menemukannya kita ambil data company nya 
                AplikasiUser = _unitOfwork.AplikasiUser.Get(u => u.Id == userId, includePropertis:"company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = _unitOfwork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text= i.Name,
                    Value = i.Id.ToString() 
                }),
            };
            RoleVM.AplikasiUser.Role = _userManager.GetRolesAsync(_unitOfwork.AplikasiUser.Get(u=>u.Id==userId)).GetAwaiter()
                .GetResult().FirstOrDefault();
            return View(RoleVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagemenVM roleManagemenVM)
        {
            var RoleLama = _userManager.GetRolesAsync(_unitOfwork.AplikasiUser.Get(u => u.Id == roleManagemenVM.AplikasiUser.Id)).GetAwaiter()
                .GetResult().FirstOrDefault();
            AplikasiUser aplikasi = _unitOfwork.AplikasiUser.Get(u => u.Id == roleManagemenVM.AplikasiUser.Id);
            if (!(roleManagemenVM.AplikasiUser.Role == RoleLama))
            {
                //update role 
                if(roleManagemenVM.AplikasiUser.Role == SD.Role_Company)
                {
                    aplikasi.CompanyId = roleManagemenVM.AplikasiUser.CompanyId;
                }
                if(RoleLama == SD.Role_Company)
                {
                    aplikasi.CompanyId = null;
                }
                _unitOfwork.AplikasiUser.Update(aplikasi);
                _unitOfwork.Save();
                _userManager.RemoveFromRoleAsync(aplikasi, RoleLama).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(aplikasi, roleManagemenVM.AplikasiUser.Role).GetAwaiter().GetResult();
            }
            else
            {
                if(RoleLama == SD.Role_Company && aplikasi.CompanyId != roleManagemenVM.AplikasiUser.CompanyId)
                {
                    aplikasi.CompanyId = roleManagemenVM.AplikasiUser.CompanyId;
                    _unitOfwork.AplikasiUser.Update(aplikasi);
                    _unitOfwork.Save(); 
                }
            }

            return RedirectToAction("Index");
        }

        #region API CALLS 
        [HttpGet]
        public IActionResult GetAll()
        {
            List<AplikasiUser> objUserList = _unitOfwork.AplikasiUser.GetAll(includePropertis:"company").ToList();

            foreach (var item in objUserList)
            {
                //setelah menemukan roleId maka kita bandingkan dengan column id pada table roles dan mengambil nama role nya 
                item.Role = _userManager.GetRolesAsync(item).GetAwaiter().GetResult().FirstOrDefault();

                if (item.company == null)
                {
                    item.company = new Company { Name = "" };
                }
            }
            return Json(new {data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _unitOfwork.AplikasiUser.Get(u => u.Id == id);
            if(objFromDb == null)
            {
				return Json(new { success = false, message = "Error Terkunci/Tidak terkunci" });
			}
            if(objFromDb.LockoutEnd !=null && objFromDb.LockoutEnd>DateTime.Now)
            {
                //pengguna tekunci dan harus dibuka 
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
            _unitOfwork.AplikasiUser.Update(objFromDb);
            _unitOfwork.Save();
            return Json(new { success = true, message = "Operation Succesfull" });
        }

        #endregion
    }
}
