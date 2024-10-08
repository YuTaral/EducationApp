﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EducationApp.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using EducationApp.Data.Services.Subjects;
using EducationApp.Data.Services.User;
using eUni.Data;

namespace EducationApp.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;


        public LoginModel(SignInManager<User> signInManager, IUserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

    
        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }

      
        public class InputModel
        {
         
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            // Check if account is approved if the user is not admin
            if (!await _userService.IsUserAdmin(Input.Email))
            {
                var isApproved = _userService.IsAccountApproved(Input.Email);

                if (isApproved == -1)
                {
                    ModelState.AddModelError(string.Empty, "Wrong username or password.");
                    return Page();
                } else if (isApproved == 0) {
                    return ViewComponent("ReturnCustomError", new { errorTitle = DataConstants.Unauthorized, errorMessage = DataConstants.ErrorAccountNotApproved });
                }
            }


            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private IActionResult ReturnCustomError(string message, string title)
        {
            var error = new CustomErrorModel
            {
                ErrorTitle = title,
                ErrorMessage = message
            };

            return ViewComponent("ReturnCustomError", error);
        }
    }
}
