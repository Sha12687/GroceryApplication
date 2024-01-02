using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDeliveryApplicationUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ICustomerRepository customerRepository;
        private readonly OrderRepository orderRepository;
        private readonly IAdminRepository adminRepository;


        // GET: Admin
        public AdminController(ICustomerRepository customerRepository,OrderRepository orderRepository,IAdminRepository adminRepository)
        {
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
            this.adminRepository = adminRepository;
        }

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult ViewCustomers()
        {
            var customers = customerRepository.GetAllCustomers();
            var customerViewModels = customers.Select(MapToViewModel).ToList();
            return View(customerViewModels);
        }


        public ActionResult ViewOrderedProducts(int customerId)
        {
            int userId = (int)Session["UserId"];
            var orders = orderRepository.GetOrdersByCustomerId(customerId);
            var productViewModels = orders
           .SelectMany(od => od.OrderDetails.Select(detail => new OrderDetailsViewModel
           {
               OrderDate = detail.Order.OrderDate,
               ProductId = detail.OrderId,
               ProductName = detail.ProductName,
               Quantity = detail.Quantity,
               Price = detail.price
           }))
           .ToList();


            return View(productViewModels);
        }

        public ActionResult ViewCustomerDetails(int customerId)
        {
            var customer = customerRepository.GetCustomerById(customerId);
            var customerModel = MapToViewModel(customer);

            return View(customerModel);
        }


        public ActionResult ViewProfile(int AdminId)
         {
            UserView viewProfile = GetAdminProfile(AdminId);
            return View(viewProfile);
        }


        public UserView GetAdminProfile(int AdminId)
        {
            var customerProfile = adminRepository.GetAdminById(AdminId);
            var viewProfile = new UserView
            {
                Id = customerProfile.Id,
                FirstName = customerProfile.FirstName,
                LastName = customerProfile.LastName,
                UserName = customerProfile.UserName,
                Email = customerProfile.Email,
                PhoneNumber = customerProfile.PhoneNumber,
            };
            return viewProfile;
        }



        public ActionResult EditAdmin(int customerId)
        {
            Admin admin = adminRepository.GetAdminById(customerId);
            var customer = new EditUserView
            {
                Id = admin.Id,
                FirstName = admin.FirstName,
                LastName = admin.LastName,
                UserName = admin.UserName,
                Email = admin.Email,
                PhoneNumber = admin.PhoneNumber,
            };
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAdmin(EditUserView User)
        {

            if (adminRepository.AdminExistsEmail(User.Email, User.Id))
            {
                // Email already registered
                ModelState.AddModelError("Email", "Email already registered with us.");
            }
            if (!ModelState.IsValid)
            {
                // Return the same view with validation errors
                return View("EditAdmin", User);
            }

            Admin ToUpdateProfile = adminRepository.GetAdminById(User.Id);
            if (ToUpdateProfile != null)
            {
                ToUpdateProfile.FirstName = User.FirstName;
                ToUpdateProfile.LastName = User.LastName;
                ToUpdateProfile.Email = User.Email;
                ToUpdateProfile.PhoneNumber = User.PhoneNumber;
                adminRepository.SaveAdminchages();
            }

            return RedirectToAction("ViewProfile", "Admin", new { AdminId = ToUpdateProfile.Id });
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
                ConfirmPassword = customer.Password  
            };
        }

    }
}