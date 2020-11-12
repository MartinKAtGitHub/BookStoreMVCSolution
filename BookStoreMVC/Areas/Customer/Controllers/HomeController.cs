using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStoreMVC.Models.ViewModels;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IO.Compression;
using BookStoreMVC.Utility;
using Microsoft.AspNetCore.Http;

namespace BookStoreMVC.Areas.Customer.Controllers
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
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            var userID = GetUserId();
            if (userID != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == userID).ToList().Count();

                HttpContext.Session.SetObject(SD.SessionNameShoppingCart, count);
                //HttpContext.Session.SetInt32(SD.SessionNameShoppingCart, count);
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeProperties: "Category,CoverType");

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                ProductId = product.Id,
                Product = product
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;

            if (ModelState.IsValid)
            {
                // Add to cart
                shoppingCart.ApplicationUserId =  GetUserId();

                var shoppingCartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == shoppingCart.ApplicationUserId && u.ProductId == shoppingCart.ProductId,
                    includeProperties: "Product"
                    );

                if (shoppingCartFromDb == null)
                {
                    // No records exists in the db for that product for that user.
                    _unitOfWork.ShoppingCart.Add(shoppingCart);
                }
                else
                {
                    shoppingCartFromDb.Count += shoppingCart.Count;
                    /*
                     * NOTE You don't need to update like this. 
                     * shoppingCartFromDb is tracked by EF core beacuse you retrived it from the DB.
                     * As long as you save the changes it will be updated in the DB.
                     * BUT consistency and readability is key in coding, i leave it as an example
                     */

                    //_unitOfWork.ShoppingCart.Update(shoppingCartFromDb); 
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();

                HttpContext.Session.SetObject(SD.SessionNameShoppingCart, count);
               
                //HttpContext.Session.SetInt32(SD.SessionNameShoppingCart, count);

                //HttpContext.Session.SetObject(SD.SessionNameShoppingCart, shoppingCart);
                //var obj = HttpContext.Session.GetObject<ShoppingCart>(SD.SessionNameShoppingCart);

                return RedirectToAction(nameof(Index));

            }
            else
            {
                var product = _unitOfWork.Product.
                    GetFirstOrDefault(p => p.Id == shoppingCart.ProductId, includeProperties: "Category,CoverType");

                ShoppingCart newShoppingCart = new ShoppingCart()
                {
                    ProductId = product.Id,
                    Product = product
                };

                return View(newShoppingCart);
            }
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

        private string GetUserId()
        {
            // Add to cart
            //NOTE Finds the current User
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? claim.Value : null;
        }
    }
}
