using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDeliveryApplicationUI.Controllers
{
    public class CustomerController : Controller
    {
        private readonly FoodDbContext _context;
        public CustomerController()
        {
            _context = new FoodDbContext();
        }
        // GET: Customer
        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            var productViewModels = products.Select(MapToViewModel).ToList();
            return View(productViewModels);
        }
        private ProductViewModel MapToViewModel(Product product)
        {
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName
                // Add other properties as needed
            };
        }
        public ActionResult AddToCart(int productId)
        {
            var userId = Session["UserId"] ;
            if (userId == null)
            {
                ModelState.AddModelError("errorMessage", "User session not found. Please log in.");
                return RedirectToAction("Index", "Customer");
            }

            var cart = _context.Products.Find(productId);

            if (cart == null)
            {
                ModelState.AddModelError("errorMessage", "Product not found.");
                return RedirectToAction("Index", "Customer");
            }

            var addTocart = new Cart
            {
                ProductName = cart.Name,
                Quantity = 1,
                ImageFileName = cart.ImageFileName,
                Price = cart.Price,
                CusomerId = Convert.ToInt32(userId)
                
            };
            if (ModelState.IsValid)
            {
                // Add the item to the database or perform other business logic
                _context.Carts.Add(addTocart);
                _context.SaveChanges();
                return RedirectToAction("Index", "Customer");
            }
            return RedirectToAction("Index", "Customer");
        }
    }
}