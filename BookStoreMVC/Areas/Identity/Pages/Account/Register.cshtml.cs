﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BookStoreMVC.DataAccess.Repository.IRepository;
using BookStoreMVC.Models;
using BookStoreMVC.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace BookStoreMVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }
            
            [Display(Name = "Street address")]
            public string StreetAddress { get; set; }
            
            public string City { get; set; }
            
            public string State { get; set; }
            
            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }
            
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
            
            public int? CompanyId { get; set; }
            
            public string Role { get; set; }

            public IEnumerable<SelectListItem> CompanyList { get; set; }

            public IEnumerable<SelectListItem> RoleList { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_User_Individual).
                Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };

            if (User.IsInRole(SD.Role_User_Employee))
            {
                Input.RoleList = _roleManager.Roles.Where(u => u.Name == SD.Role_User_Company).
               Select(x => x.Name).Select(i => new SelectListItem
               {
                   Text = i,
                   Value = i
               });
            }

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    CompanyId = Input.CompanyId,
                    StreetAddress = Input.StreetAddress,
                    City = Input.City,
                    State = Input.State,
                    PostalCode = Input.PostalCode,
                    Name = Input.Name,
                    PhoneNumber = Input.PhoneNumber,
                    Role = Input.Role
                };
                
                var result = await _userManager.CreateAsync(user, Input.Password);
               
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    if (user.Role == null) // If a role is not assigned make them a individual customer
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_User_Individual);
                    }
                    else
                    {
                        if (user.CompanyId > 0)
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_User_Company);
                        }
                        await _userManager.AddToRoleAsync(user, user.Role);
                    }

                    /* TODO : Confirmation Email - I can implement a 3rd party E mail service to send out confirmation email.
                     * But i feel this is something which will be different from solution to solution so i will not add one in my project.
                     * The idea is simple. ASP.NET Core generates a reset token and a link. the link is what we send in a email. User follows the Link and the password will be reset.
                     * How we send the Email is the problem, SendGrid is a pain and other systems are a little suspect. I have my own solution to this but it is not secure.
                     */

                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");


                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // A regular person dose not have the ability to give them self a role so this field will be NULL
                        if (user.Role == null)
                        {
                            // sign inn and redirect
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            //Admin/Employee has assigned a role
                            return RedirectToAction("index", "User", new { Area = "Admin" });
                        }

                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Input = new InputModel()
            {
                CompanyList = _unitOfWork.Company.GetAll()
                .Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),

                RoleList = _roleManager.Roles.Where(u => u.Name != SD.Role_User_Individual)
                .Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
