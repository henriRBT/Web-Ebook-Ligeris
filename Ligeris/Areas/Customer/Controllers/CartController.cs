using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Modelss;
using Ligeris.Modelss.ViewModels;
using Ligeris.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ligeris.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ShoopingCardVM ShoopingCardVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoopingCardVM = new()
            {
                ShoopingCartList = _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == userId,
                includePropertis: "Product"),
                OrderHeader = new()
            };
			IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
			foreach (var cart in ShoopingCardVM.ShoopingCartList)
            {
				cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
				cart.Harga = GetHargaQuantity(cart);
                ShoopingCardVM.OrderHeader.TotalOrder += (cart.Harga * cart.Count);
            }
            return View(ShoopingCardVM);
        }

        public IActionResult Pesan()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoopingCardVM = new()
            {
                ShoopingCartList = _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == userId,
                includePropertis: "Product"),
                OrderHeader = new()
            };

            ShoopingCardVM.OrderHeader.AplikasiUser = _unitOfWork.AplikasiUser.Get(u => u.Id == userId);
            ShoopingCardVM.OrderHeader.Name = ShoopingCardVM.OrderHeader.AplikasiUser.Name;
            ShoopingCardVM.OrderHeader.nomorHp = ShoopingCardVM.OrderHeader.AplikasiUser.PhoneNumber;
            ShoopingCardVM.OrderHeader.alamat = ShoopingCardVM.OrderHeader.AplikasiUser.StreetAddress;
            ShoopingCardVM.OrderHeader.kota = ShoopingCardVM.OrderHeader.AplikasiUser.City;
            ShoopingCardVM.OrderHeader.negara = ShoopingCardVM.OrderHeader.AplikasiUser.State;
            ShoopingCardVM.OrderHeader.kodePos = ShoopingCardVM.OrderHeader.AplikasiUser.PostalCode;

            foreach (var cart in ShoopingCardVM.ShoopingCartList)
            {
                cart.Harga = GetHargaQuantity(cart);
                ShoopingCardVM.OrderHeader.TotalOrder += (cart.Harga * cart.Count);
            }
            return View(ShoopingCardVM);
        }

        [HttpPost]
        [ActionName("Pesan")]
        public IActionResult PesanPost(ShoopingCardVM shoopingCardVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoopingCardVM.ShoopingCartList = _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == userId,
                includePropertis: "Product");

            ShoopingCardVM.OrderHeader.TanggalOrder = System.DateTime.Now;
            ShoopingCardVM.OrderHeader.AplikasiUserId = userId;

            AplikasiUser useraplikasi = _unitOfWork.AplikasiUser.Get(u => u.Id == userId);

            foreach (var cart in ShoopingCardVM.ShoopingCartList)
            {
                cart.Harga = GetHargaQuantity(cart);
                ShoopingCardVM.OrderHeader.TotalOrder += (cart.Harga * cart.Count);
            }

            //Pengecekan Perusahaan 
            if (useraplikasi.CompanyId.GetValueOrDefault() == 0)
            {
                //ini bagian untuk customer biasa yang tidak ada perusahaan 
                ShoopingCardVM.OrderHeader.StatusPembayaran = SD.PaymentStatusPending;
                ShoopingCardVM.OrderHeader.StatusOrder = SD.StatusPending;
            }
            else
            {
                //ini untuk perusahaan 
                ShoopingCardVM.OrderHeader.StatusPembayaran = SD.PaymentStatusDelayedPayment;
                ShoopingCardVM.OrderHeader.StatusOrder = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(ShoopingCardVM.OrderHeader);
            _unitOfWork.Save();
            //ini untuk detail pesanan
            foreach (var cart in ShoopingCardVM.ShoopingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoopingCardVM.OrderHeader.Id,
                    Harga = cart.Harga,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (useraplikasi.CompanyId.GetValueOrDefault() == 0)
            {
                //ini bagian untuk customer biasa yang tidak ada perusahaan 
                var domain = "https://localhost:44385/";
				var options = new Stripe.Checkout.SessionCreateOptions
				{
					SuccessUrl = domain+ $"customer/cart/OrderConfimasi?id={ShoopingCardVM.OrderHeader.Id}",
                    CancelUrl = domain +"customer/cart/index",
					LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach(var item in ShoopingCardVM.ShoopingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Harga * 100),
                            Currency = "idr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
				}

                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymenID(ShoopingCardVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
			}
            return RedirectToAction(nameof(OrderConfimasi), new {id=ShoopingCardVM.OrderHeader.Id});
        }

        public IActionResult OrderConfimasi(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u =>u.Id == id, includePropertis: "AplikasiUser");
            if(orderHeader.StatusPembayaran != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
					_unitOfWork.OrderHeader.UpdateStripePaymenID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
                HttpContext.Session.Clear();
			}
            _emailSender.SendEmailAsync(orderHeader.AplikasiUser.Email, "New Order - Lingeris Book", $"<p> New Order Create - {orderHeader.Id}</p>");

            List<ShoopingCard> shoopingCards = _unitOfWork.Shooping
                .GetAll(u=>u.ApplicationUserId == orderHeader.AplikasiUserId).ToList();
            _unitOfWork.Shooping.RemoveRange(shoopingCards);
            _unitOfWork.Save();
			return View(id);
        }

		public IActionResult tambah(int cartId)
        {
            var hargaFromDb = _unitOfWork.Shooping.Get(u=>u.Id== cartId);
            hargaFromDb.Count += 1;
            _unitOfWork.Shooping.Update(hargaFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult kurang(int cartId)
        {
            var cartfromDb = _unitOfWork.Shooping.Get(u => u.Id == cartId, tracked: true);
            if(cartfromDb.Count <= 1)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == cartfromDb.ApplicationUserId).Count() - 1);
                _unitOfWork.Shooping.Remove(cartfromDb);
            }
            else
            {
                cartfromDb.Count -= 1;
                _unitOfWork.Shooping.Update(cartfromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult hapus(int cartId)
        {
            var cartfromDb = _unitOfWork.Shooping.Get(u => u.Id == cartId, tracked:true);
            HttpContext.Session.SetInt32(SD.SessionCart,
               _unitOfWork.Shooping.GetAll(u => u.ApplicationUserId == cartfromDb.ApplicationUserId).Count() - 1);
            _unitOfWork.Shooping.Remove(cartfromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        private double GetHargaQuantity(ShoopingCard shoopingCard)
        {
            if(shoopingCard.Count <= 50)
            {
                return shoopingCard.Product.Price;
            }
            else
            {
                if(shoopingCard.Count <= 100)
                {
                    return shoopingCard.Product.Price100;
                }
                else
                {
                    return shoopingCard.Product.Price500;
                }
            }
        }

    }
}
