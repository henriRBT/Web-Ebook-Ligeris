using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ligeris.ViewComponents
{
    public class ShoopingCardViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoopingCardViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) ==null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
