using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Modelss;
using Ligeris.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Ligeris.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includePropertis:"Category");
            return View(productList);
        }

        public IActionResult Details(int productId)
        {
            ShoopingCard card = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includePropertis: "Category"),
                Count = 1,
                ProductId = productId
            };
			
			return View(card);
		}

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoopingCard shoopingCard)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            shoopingCard.ApplicationUserId = userId;

            ShoopingCard cardDb = _unitOfWork.Shooping.Get(u => u.ApplicationUserId == userId && u.ProductId == shoopingCard.ProductId);
            if (cardDb != null)
            {
                cardDb.Count += shoopingCard.Count;
                _unitOfWork.Shooping.Update(cardDb);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
               _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == userId).Count());

            }
            else
            {
                _unitOfWork.Shooping.Add(shoopingCard);
                _unitOfWork.Save();
                //ini ditambahkan saat customer menambah belanja 
                //alasan kita menambahkannya jumlah keranjang di session agar mendeteksi jumlah barang di keranjang dan logout secara otomatis nantinya di 
                //keranjang
                HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Successfully tambahan belanja";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
