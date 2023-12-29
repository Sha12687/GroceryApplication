using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDeliveryApplicationUI.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CustomerController : Controller
    {
        //  private readonly FoodDbContext _context;
        private readonly ICustomerRepository customerRepository;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IOrderDetailRepository orderDetailRepository;

        public CustomerController(ICustomerRepository customerRepository, IProductRepository productRepository, ICartRepository cartRepository, IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            //   _context = new FoodDbContext();
            this.customerRepository = customerRepository;
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
            this.orderRepository = orderRepository;
            this.orderDetailRepository = orderDetailRepository;
        }
        // GET: Customer
        public ActionResult Index()
        {
            var products = productRepository.GetAllProducts();
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

            var cart = productRepository.GetProductById(productId);

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
                cartRepository.CreateCartItem(addTocart);

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
            int temp = Convert.ToInt32(userId);
            var existingCartItem = customerRepository.GetCartItemByProductIdAndCustomerId(productId, temp);

            if (existingCartItem != null)
            {
                // Product already exists in the cart, so update the quantity
                existingCartItem.Quantity++;
                cartRepository.cartSaveChanges();
            }
            else
            {
                // Product is not in the cart, so add a new item
                var cart = productRepository.GetProductById(productId);

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

                cartRepository.CreateCartItem(addTocart);

            }


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

            var cartItems = cartRepository.GetCartItemsByCustomerId(customerId);
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
            var cartItem = cartRepository.GetCartItemById(cartId);
            if (cartItem != null)
            {
                // Update the quantity
                cartItem.Quantity = newQuantity;

                cartRepository.cartSaveChanges();
            }
            return View();
        }

        public ActionResult PlaceOrder(int[] cartIds, int? customerId)
        {

            if (customerId == null)
            {
                var existingOrder = Session["OrderViewModel"] as OrderViewModel;
                return View(existingOrder);
            }
            else
            {
                int custId = Convert.ToInt32(customerId);
                var cartItems = cartRepository.GetCartItemById(cartIds);
                var customer = customerRepository.GetCustomerById(custId);

                var order = new OrderViewModel
                {
                    name = customer.UserName,
                    CustId = customer.Id,
                    UserName = customer.UserName,
                    OrderDate = DateTime.Now,
                    OrderDetails = cartItems.Select(item => new OrderDetailsViewModel
                    {
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Subtotal = item.Quantity * item.Price,
                        ProductId = item.ProductId,
                    }).ToList()
                };
                decimal total = cartItems.Sum(item => item.Quantity * item.Price);
                order.TotalAmount = total;
                Session["OrderViewModel"] = order;

                // Other logic (e.g., payment processing) can be added here

                return View(order);
            }
        }

        [HttpPost]
        public ActionResult OrderConfirm(int customerId)
        {
            // Retrieve cart items
            var cartItems = cartRepository.GetCartItemsByCustomerId(customerId);
            var customer = customerRepository.GetCustomerById(customerId);

            // Create an order
            var order = new Order
            {
                OrderDate = DateTime.Now,
                CustomerId = customer.Id,
                TotalAmount = cartItems.Sum(item => item.Quantity * item.Price),
                OrderDetails = cartItems.Select(item => new OrderDetail
                {
                    ProductName = item.ProductName,
                    price = item.Price,
                    Quantity = item.Quantity
                }).ToList()
            };

            // Save the order
            orderRepository.CreateOrder(order);

            // Remove cart items
            foreach (var cartItem in cartItems)
            {
                cartRepository.DeleteCartItem(cartItem.CartId);
            }

            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        public ActionResult OrderConfirmation(int orderId)
        {
            // Retrieve the order for confirmation
            var order = orderRepository.GetOrderById(orderId);
            var message = new OrderViewModel
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
            };
            return View(message);
        }

        public ActionResult OrderPlacedSuccessfully()
        {
            return View();
        }

        public ActionResult ViewOrderedProducts()
        {
            int userId = (int)Session["UserId"];
            var orders = orderRepository.GetOrdersByCustomerId(userId);
            var productViewModels = orders
           .SelectMany(od => od.OrderDetails.Select(detail => new OrderDetailsViewModel
           {
               OrderDate=detail.Order.OrderDate,
               ProductId = detail.OrderId,
               ProductName = detail.ProductName,
               Quantity = detail.Quantity,
               Price = detail.price
           }))
           .ToList();
            return View(productViewModels);
        }
        public ActionResult ViewProfile(int customerId)
        {
            CustomerModel viewProfile = GetCustomerProfile(customerId);
            return View(viewProfile);
        }

        public ActionResult EditCustomer(int customerId)
        {
            CustomerModel customer = GetCustomerProfile(customerId);
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCustomer(CustomerModel customer)
        {
            Customer ToUpdateProfile = customerRepository.GetCustomerById(customer.Id);
            if(ToUpdateProfile != null)
            {
                ToUpdateProfile.FirstName = customer.FirstName;
                ToUpdateProfile.LastName = customer.LastName;
                ToUpdateProfile.Email = customer.Email;
                ToUpdateProfile.PhoneNumber = customer.PhoneNumber;
                ToUpdateProfile.UserName = customer.UserName;
                customerRepository.customerSAveChanges();

            }
            return RedirectToAction("ViewProfile", "Customer", new { customerId = ToUpdateProfile.Id });
        }

        
        public CustomerModel GetCustomerProfile(int customerId)
        {
            var customerProfile = customerRepository.GetCustomerById(customerId);
            var viewProfile = new CustomerModel
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

       
    }
}