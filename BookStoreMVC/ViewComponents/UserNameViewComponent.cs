using BookStoreMVC.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStoreMVC.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public UserNameViewComponent(IUnitOfWork  unitOfWork , UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task <IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId((ClaimsPrincipal)User);
            var userFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == userId);
            return View(userFromDb);
        }
    }
}
