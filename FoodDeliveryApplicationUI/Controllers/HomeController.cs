using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDeliveryApplicationUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        private CustomerModel MapToViewModel(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                UserName = customer.UserName,
                Password = customer.Password,
                ConfirmPassword = customer.Password  // Assuming you store hashed passwords
                                                     // Add other properties as needed
            };
        }
    }
}