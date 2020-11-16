using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using BookStoreMVC.Models.ViewModels;
using BookStoreMVC.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreMVC.Areas.Customer.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Administrator)]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int productPage = 1)
        {
            CategoryViewModel categoryViewModel = new CategoryViewModel()
            {
                Categories = _unitOfWork.Category.GetAll()
            };

            var categoriesPerPage = 2;
            var count = categoryViewModel.Categories.Count();
            categoryViewModel.Categories = categoryViewModel.Categories.OrderBy(p => p.Name)
                .Skip((productPage - 1) * categoriesPerPage).Take(categoriesPerPage).ToList();

            categoryViewModel.PageInfo = new PageInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage = 2,
                TotalItem = count,
                UrlParam = "/Admin/Category/Index?productPage=:" // the ':' is the character i target in my tag helper (PageLinkTagHelper) Line 37
            };

            return View(categoryViewModel);
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();

            if (id == null)
            {
                // Crate a new category
                return View(category);
            }

            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    _unitOfWork.Category.Update(category);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryFromDb = _unitOfWork.Category.Get(id);
            if (categoryFromDb == null)
            {
                TempData["Error"] = "Error deleting category";
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();

            TempData["Success"] = "Category successfully deleted";
            return Json(new { success = true, message = $"{categoryFromDb.Name} was deleted" });

        }

        #endregion
    }
}
