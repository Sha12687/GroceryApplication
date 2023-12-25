using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Service;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FoodDeliveryApplicationUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly FoodDbContext context;

        public AccountController()
        {
            this.context = new FoodDbContext();
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
        private bool AuthenticateAdmin(string username, string password)
        {

            var admin = context.Admins.SingleOrDefault(a => a.UserName == username && a.Password == password);

            // Return true if an admin is found, otherwise false.
            return admin != null;
        }
        [HttpPost]
        public ActionResult CustomerLogin(LoginViewModel loginView)
        {
            var isAdmin = Authentication.VerifyCustomerCredentials(loginView.UserName, loginView.Password);

            if (isAdmin)
            {
           var user = context.Customers.FirstOrDefault(x => x.UserName == loginView.UserName);
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
            if (context.Admins.Any(a => a.Email == user.Email) || context.Customers.Any(e => e.Email == user.Email))
            {
                // Email already registered
                ModelState.AddModelError("Email", "Email already registered with us.");
                return View("Registration", user);
            }
            else if (context.Admins.Any(a => a.UserName == user.UserName) || context.Customers.Any(e => e.UserName == user.UserName))
            {
                // Username already registered
                ModelState.AddModelError("UserName", "Username already registered with us.");

                return View("Registration", user);
            }
            if (user.UserType == 2)
            {
                Customer employee = new Customer
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    RoleId = user.UserType
                };
                var passwordHash = new PasswordHasher<Customer>();
                employee.Password = passwordHash.HashPassword(employee, user.Password);
                context.Customers.Add(employee);
                context.SaveChanges();

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
                context.Admins.Add(newadmin);
                context.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
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