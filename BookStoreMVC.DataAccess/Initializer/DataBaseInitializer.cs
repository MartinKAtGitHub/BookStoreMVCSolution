using BookStoreMVC.DataAccess.Data;
using BookStoreMVC.Models;
using BookStoreMVC.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookStoreMVC.DataAccess.Initializer
{
    public class DataBaseInitializer : IDataBaseInitializer
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataBaseInitializer(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initializer()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    // This will Create the Database if it dose not already exist And apply all the pending migrations
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception)
            {
                // TODO add logging in DB Initializer
                throw;
            }

            if(_dbContext.Roles.Any(r => r.Name == SD.Role_User_Administrator)){
                return;
            }

            // Generate All the roles the webapp is using
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Administrator)).GetAwaiter().GetResult(); // Waits until this is crated
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Employee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Individual)).GetAwaiter().GetResult();


            var newAdmin = new ApplicationUser
            {
                Email = "Admin@Admin.com",
                EmailConfirmed = true,
                Name = "Admin Name"
            };
            newAdmin.UserName = newAdmin.Email; // The system is set up to use the same thing

            var result =_userManager.CreateAsync(newAdmin, "Admin_123").GetAwaiter().GetResult();

            ApplicationUser user = _dbContext.ApplicationUsers.Where(u => u.Email == newAdmin.Email).FirstOrDefault();

            _userManager.AddToRoleAsync(user, SD.Role_User_Administrator).GetAwaiter().GetResult();

        }
    }
}
