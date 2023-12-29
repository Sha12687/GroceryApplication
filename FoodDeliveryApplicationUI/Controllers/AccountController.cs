using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Service;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FoodDeliveryApplicationUI.Controllers
{
    public class AccountController : Controller
    {
      
        private readonly IAdminRepository adminRepository;
        private readonly ICustomerRepository customerRepository;

        public AccountController(IAdminRepository adminRepository,ICustomerRepository customerRepository)
        {
            this.adminRepository = adminRepository;
            this.customerRepository = customerRepository;
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            return View();
        }
        // GET: Account
        public ActionResult CustomerLogin()
        {
            return View();
        }

        public ActionResult AdminLogin()
        {


            return View();
        }

        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminLogin(LoginViewModel loginView)
        {
            var isAdmin = Authentication.VerifyAdminCredentials(loginView.UserName, loginView.Password);

            if (isAdmin)
            {
                var user = adminRepository.GetAdminByUserName(loginView.UserName);
                Session["UserId"] = user.Id;
                Session["UserName"] = user.UserName;
                FormsAuthentication.SetAuthCookie(loginView.UserName, false);
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // If authentication fails, you may want to show an error message.
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(loginView);
            }
        }
        [HttpPost]
        public ActionResult CustomerLogin(LoginViewModel loginView)
        {
            var isAdmin = Authentication.VerifyCustomerCredentials(loginView.UserName, loginView.Password);

            if (isAdmin)
            {
           var user = customerRepository.GetCustomerByUserName( loginView.UserName);
                Session["UserId"] = user.Id;
                Session["UserName"] = user.UserName;
                FormsAuthentication.SetAuthCookie(loginView.UserName, false);
                return RedirectToAction("Index", "Customer");
            }
            else
            {
                // If authentication fails, you may want to show an error message.
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(loginView);
            }
        }

        [HttpPost]
        public ActionResult Registration(UserView user)
        {
            if (adminRepository.AdminExistsEmail( user.Email) || customerRepository.CustomerExistsEmail( user.Email))
            {
                // Email already registered
                ModelState.AddModelError("Email", "Email already registered with us.");
                return View("Registration", user);
            }
            else if (adminRepository.AdminExists(user.UserName) || customerRepository.CustomerExists( user.UserName))
            {
                // Username already registered
                ModelState.AddModelError("UserName", "Username already registered with us.");

                return View("Registration", user);
            }
            if (user.UserType == 2)
            {
                Customer customer = new Customer
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.UserType
                };
                var passwordHash = new PasswordHasher<Customer>();
                customer.Password = passwordHash.HashPassword(customer, user.Password);
                customerRepository.CreateCustomer(customer);
             

                return RedirectToAction("Index", "Customer");
            }
            else
            {
                Admin newadmin = new Admin
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.UserType
                };
                var passwordHash = new PasswordHasher<Admin>();
                newadmin.Password = passwordHash.HashPassword(newadmin, user.Password);
                adminRepository.CreateAdmin(newadmin);
            
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
               var user = customerRepository.GetCustomerByUserName(model.UserName);

                    if (user == null)
                    {
                        ModelState.AddModelError(nameof(model.UserName), "Invalid username. Please enter a valid username.");
                        return View(model);
                    }
                    else
                    {
                        var passwordHash = new PasswordHasher<Customer>();
                        user.Password = passwordHash.HashPassword(user, model.Password);
                        customerRepository.customerSAveChanges();
                    }
                    TempData["SuccessMessage"] = "Password reset successfully. Please log in with your new password.";
                    return RedirectToAction("CustomerLogin", "Account");
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("CustomerLogin", "Account");
        }
    }
}