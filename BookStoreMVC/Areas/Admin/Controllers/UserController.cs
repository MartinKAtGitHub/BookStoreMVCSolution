using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStoreMVC.Areas.Customer.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork; // Trying to use the more direct way for testing purposes, obviously i wont do this in a real Project
        private readonly ApplicationDbContext _dbContext;


        public UserController(/*IUnitOfWork unitOfWork*/ ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            //_unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = _dbContext.applicationUsers.Include(u => u.Company).ToList();
            var userRoles = _dbContext.UserRoles.ToList();
            var roles = _dbContext.Roles.ToList();
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u=> u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            
            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var objFromDb = _dbContext.applicationUsers.FirstOrDefault(u => u.Id == id);
           
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            
            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user IS currently locked, we will unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                // user in NOT locked. Lock em.
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _dbContext.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });

        }

        #endregion
    }
}
