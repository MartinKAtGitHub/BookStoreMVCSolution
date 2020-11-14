using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using BookStoreMVC.Models.ViewModels;
using BookStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public OrderDetailsViewModel OrderDetailsViewModel { get; set; }

        public OrderController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id, includeProperties: "ApplicationUser"),
                OrderDetailsObjects = _unitOfWork.OrderDetails.GetAll(o => o.OrderId == id, includeProperties: "Product")
            };

            return View(orderDetailsViewModel);
        }


        [Authorize(Roles = SD.Role_User_Administrator + "," + SD.Role_User_Employee)]
        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = SD.Status_InProcess;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_User_Administrator + "," + SD.Role_User_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderDetailsViewModel.OrderHeader.Id);

            orderHeader.TrackingNumber = OrderDetailsViewModel.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderDetailsViewModel.OrderHeader.Carrier;
            orderHeader.OrderStatus = SD.Status_Shipped;
            orderHeader.ShippingDate = DateTime.Now;

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = SD.Role_User_Administrator + "," + SD.Role_User_Employee)]
        public IActionResult CancelOrder(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            // Add Refund Logic here -- start -- 
            //if(payment was approved)
            // Refund
            // Set refund statues
            // ELSE
            orderHeader.OrderStatus = SD.Status_Canceled;
            orderHeader.PaymentStatus = SD.Status_Canceled;
            //-- end -- 

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var userId = _userManager.GetUserId(User);
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_User_Administrator) || User.IsInRole(SD.Role_User_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.Status_Approved || o.OrderStatus == SD.Status_InProcess || o.OrderStatus == SD.Status_Pending);
                    break;
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatus_DelayedPayment);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.Status_Shipped);
                    break;
                case "rejected":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.Status_Canceled || o.OrderStatus == SD.Status_Refunded || o.OrderStatus == SD.PaymentStatus_Rejected);
                    break;
                default:
                    break;
            }
            return Json(new { data = orderHeaders });
        }
        #endregion
    }


}
