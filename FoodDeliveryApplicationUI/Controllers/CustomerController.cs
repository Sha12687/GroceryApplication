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
        public ActionResult AddToCart2(int productId)
        {
            var userId = Session["UserId"];
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
        public ActionResult AddToCart(int productId)
        {
            var userId = Session["UserId"] as int?;
            if (userId == null)
            {
                ModelState.AddModelError("errorMessage", "User session not found. Please log in.");
                return RedirectToAction("Index", "Customer");
            }

            var existingCartItem = _context.Carts
                .FirstOrDefault(c => c.ProductId == productId && c.CusomerId == userId);

            if (existingCartItem != null)
            {
                // Product already exists in the cart, so update the quantity
                existingCartItem.Quantity++;
            }
            else
            {
                // Product is not in the cart, so add a new item
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
                    CusomerId = Convert.ToInt32(userId),
                    ProductId = productId  // Add the ProductId property to your Cart model to store the product ID
                };

                _context.Carts.Add(addTocart);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Customer");
        }

        public ActionResult ViewCart(int? customerId)
        {
            // Check if customerId is provided and is a valid value
            if (customerId == null)
            {
                // You might want to redirect to a login page or show an error message
                return RedirectToAction("CustomerLogin", "Account");
            }
            var loggedInUserId = (int?)Session["UserId"];
            // Retrieve cart items for the specified customer
            var cartItems = _context.Carts.Where(c => c.CusomerId == customerId).ToList();

            // Create a list of CartViewModel to pass to the view
            var carViewList = cartItems.Select(item => new CartViewModel
            {
                CartId = item.CartId,
                ImageFileName = item.ImageFileName,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price,
                CustomerId = item.CusomerId
            });

            return View(carViewList);
        }
        [HttpPost]
        public ActionResult UpdateCartQuantity(int cartId, int newQuantity)
        {
            var cartItem = _context.Carts.Find(cartId);
            if (cartItem != null)
            {
                // Update the quantity
                cartItem.Quantity =newQuantity;

                _context.SaveChanges();
            }
          return View();
        }
    }
}