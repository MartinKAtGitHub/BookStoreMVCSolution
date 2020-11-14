using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using BookStoreMVC.Models.ViewModels;
using BookStoreMVC.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            //var claimsID = (ClaimsIdentity)User.Identity;
            //var claim = claimsID.FindFirst(ClaimTypes.NameIdentifier);

            var userId = _userManager.GetUserId(User);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                //ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value),
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };

            ShoppingCartViewModel.OrderHeader.OrderTotal = 0;
            ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                 .GetFirstOrDefault(u => u.Id == userId, includeProperties: "Company");
            //.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");

            foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
            {
                cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);

                cart.Product.Description = SD.ConvertToRawHtml(cart.Product.Description);
                if (cart.Product.Description.Length > 100)
                {
                    cart.Product.Description = cart.Product.Description.Substring(0, 99) + ". . .";
                }
            }

            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ActionName("Index")] //NOTE we have to go around the the issue of parameters. Since we cant have the same Function Name and Same Parameters we have to use this
        public IActionResult IndexPOST() // TODO : Implement Sending Confirmation Email - SendGrid is scummy find another provider
        // public async Task<IActionResult> IndexPOST()
        {
            var userId = _userManager.GetUserId(User);
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Cant find User");
            }


            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var callbackUrl = Url.Page(
            //    "/Account/ConfirmEmail",
            //    pageHandler: null,
            //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
            //    protocol: Request.Scheme);

            //await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
            //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Email Confirm Not added since SENDGRID is lame");
            return RedirectToAction("Index");
        }

        public IActionResult Summary()
        {
            var userId = _userManager.GetUserId(User);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader(),
                ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product")
            };

            ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                                    .GetFirstOrDefault(u => u.Id == userId,
                                                                    includeProperties: "Company");

            foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
            {
                cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

            return View(ShoppingCartViewModel);
        }
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost(/*ShoppingCartViewModel ShoppingCartViewModel*/) // We are just going to [BindProperty] the ShoppingCarViewModel
        {
            var userId = _userManager.GetUserId(User);
            
            if (ModelState.IsValid)
            {
                ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                    .GetFirstOrDefault(u => u.Id == userId, includeProperties: "Company");

                ShoppingCartViewModel.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product");

                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_Pending;
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.Status_Pending;
                ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
                ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;

                _unitOfWork.OrderHeader.Add(ShoppingCartViewModel.OrderHeader);
                _unitOfWork.Save();

                foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
                {
                    cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                    OrderDetails orderDetailObject = new OrderDetails()
                    {
                        ProductId = cart.ProductId,
                        OrderId = ShoppingCartViewModel.OrderHeader.Id,
                        Price = cart.Price,
                        Count = cart.Count
                    };

                    ShoppingCartViewModel.OrderHeader.OrderTotal += orderDetailObject.Count * orderDetailObject.Price;
                    _unitOfWork.OrderDetails.Add(orderDetailObject);

                }
                _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartViewModel.ShoppingCarts);

                HttpContext.Session.SetObject(SD.SessionNameShoppingCart, 0);

                /*Add payment checking here --> The payment implementation would differ from workplace to workplace so i just skipped it*/
                //If(payment failed)
                //ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_Rejected;
                //Else
                ShoppingCartViewModel.OrderHeader.PaymentStatus = SD.PaymentStatus_Approved;
                ShoppingCartViewModel.OrderHeader.OrderStatus = SD.Status_Approved;
                ShoppingCartViewModel.OrderHeader.PaymentDate = DateTime.Now;
                // ---end ---

                _unitOfWork.Save();
                return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartViewModel.OrderHeader.Id });
            }
            else
            {
                ShoppingCartViewModel = new ShoppingCartViewModel()
                {
                    OrderHeader = new OrderHeader(),
                    ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == userId, includeProperties: "Product")
                };

                ShoppingCartViewModel.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                                        .GetFirstOrDefault(u => u.Id == userId,
                                                                        includeProperties: "Company");

                foreach (var cart in ShoppingCartViewModel.ShoppingCarts)
                {
                    cart.Price = SD.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                    ShoppingCartViewModel.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }

                ShoppingCartViewModel.OrderHeader.Name = ShoppingCartViewModel.OrderHeader.ApplicationUser.Name;
                ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
                ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
                ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
                ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
                ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;

                return View(ShoppingCartViewModel); // this works with [ActionName("Summary")]
               // return RedirectToAction(nameof(Summary));
            }

        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }

        public IActionResult AddProduct(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Product");

            shoppingCart.Count += 1;
            shoppingCart.Price = SD.GetPriceBasedOnQuantity(shoppingCart.Count,
                shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);

            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult SubtractProduct(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Product");

            if (shoppingCart.Count <= 1)
            {
                var numberOfshoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();

                _unitOfWork.ShoppingCart.Remove(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetObject(SD.SessionNameShoppingCart, numberOfshoppingCarts - 1);
            }
            else
            {
                shoppingCart.Count -= 1;
                shoppingCart.Price = SD.GetPriceBasedOnQuantity(shoppingCart.Count,
                    shoppingCart.Product.Price, shoppingCart.Product.Price50, shoppingCart.Product.Price100);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));

        }

        public IActionResult RemoveProduct(int cartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId, includeProperties: "Product");
            var numberOfshoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();

            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            _unitOfWork.Save();
            HttpContext.Session.SetObject(SD.SessionNameShoppingCart, numberOfshoppingCarts - 1);

            return RedirectToAction(nameof(Index));

        }
    }
}
