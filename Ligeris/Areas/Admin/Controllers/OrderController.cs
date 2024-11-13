using Ligeris.DataAcess.Repository.IRepository;
using Ligeris.Modelss;
using Ligeris.Modelss.ViewModels;
using Ligeris.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Diagnostics;
using System.Security.Claims;

namespace Ligeris.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public OrderVM orderVM { get; set; }

		public OrderController(IUnitOfWork unitOfWork) 
		{ 
			_unitOfWork= unitOfWork;
		}
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Details(int orderId)
		{
			orderVM = new()
			{
				orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includePropertis: "AplikasiUser"),
				OrderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includePropertis:"Product"),
			};
			return View(orderVM);
		}
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult updateOrderDetail()
        {
			var orderheaderDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);
            orderheaderDb.Name = orderVM.orderHeader.Name;
            orderheaderDb.nomorHp = orderVM.orderHeader.nomorHp;
			orderheaderDb.alamat = orderVM.orderHeader.alamat;
			orderheaderDb.kota = orderVM.orderHeader.kota;
			orderheaderDb.negara = orderVM.orderHeader.negara;
			orderheaderDb.kodePos = orderVM.orderHeader.kodePos;
			if (!string.IsNullOrEmpty(orderVM.orderHeader.kurir))
			{
				orderheaderDb.kurir = orderVM.orderHeader.kurir;
			}
			if (!string.IsNullOrEmpty(orderVM.orderHeader.NomorPelacakan))
			{
				orderheaderDb.kurir = orderVM.orderHeader.NomorPelacakan;
            }
			_unitOfWork.OrderHeader.Update(orderheaderDb);
			_unitOfWork.Save();
			TempData["Success"] = "Update Detail Order berhasil";

			return RedirectToAction(nameof(Details), new
			{
				orderId = orderheaderDb.Id
			});
        }
		[HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
		{
			_unitOfWork.OrderHeader.UpdateStatus(orderVM.orderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();

            TempData["Success"] = "Update Detail Order berhasil";

            return RedirectToAction(nameof(Details), new
            {
                orderId = orderVM.orderHeader.Id
            });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
			var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);
			orderHeader.NomorPelacakan = orderVM.orderHeader.NomorPelacakan;
			orderHeader.kurir = orderVM.orderHeader.kurir;
			orderHeader.StatusOrder = SD.StatusShipped;	
			orderHeader.TanggalPengiriman = DateTime.Now;
			if(orderHeader.StatusPembayaran == SD.PaymentStatusDelayedPayment)
			{
				orderHeader.JatuhTempo = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

            }
			_unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Update Order Pengiriman berhasil";

            return RedirectToAction(nameof(Details), new
            {
                orderId = orderVM.orderHeader.Id
            });
        }
        
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderVM.orderHeader.Id);
            if (orderHeader.StatusPembayaran == SD.PaymentStatusApproved)
            {
				var options = new RefundCreateOptions
				{
					Reason=RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(options);
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
			}
			else
			{
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["Success"] = "Order Cancel berhasil";

            return RedirectToAction(nameof(Details), new
            {
                orderId = orderVM.orderHeader.Id
            });
        }

		[ActionName("Details")]
		[HttpPost]
		public IActionResult DetailPay()
		{
			orderVM.orderHeader = _unitOfWork.OrderHeader
				.Get(u => u.Id == orderVM.orderHeader.Id, includePropertis: "AplikasiUser");

			orderVM.OrderDetails = _unitOfWork.OrderDetail
				.GetAll(u => u.OrderHeaderId == orderVM.orderHeader.Id, includePropertis: "Product");

            var domain = "https://localhost:44385/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?OrderHeaderId={orderVM.orderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={orderVM.orderHeader.Id}",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in orderVM.OrderDetails)
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
            _unitOfWork.OrderHeader.UpdateStripePaymenID(orderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int OrderHeaderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderHeaderId);
            if (orderHeader.StatusPembayaran == SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymenID(OrderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(OrderHeaderId, orderHeader.StatusOrder, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            return View(OrderHeaderId);
        }


        #region API CALLS 
        [HttpGet]
		public IActionResult GetAll( string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders;
			if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)){
				objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includePropertis:"AplikasiUser").ToList();
            }
			else
			{
				var claimIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                objOrderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.AplikasiUserId == userId, includePropertis: "AplikasiUser" );
            }

			switch (status)
			{
				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.StatusPembayaran == SD.PaymentStatusPending);
					break;
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.StatusOrder == SD.StatusInProcess);
					break;
				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.StatusOrder == SD.StatusShipped);
					break;
				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.StatusOrder == SD.StatusApproved);
					break;
				default:
					break;
			}

			return Json(new { data = objOrderHeaders });
		}
		
		#endregion
	}
}
