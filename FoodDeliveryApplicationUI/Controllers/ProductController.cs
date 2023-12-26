using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace FoodDeliveryApplicationUI.Controllers
{
    public class ProductController : Controller
    {
       
        private readonly FoodDbContext  _context;
        public ProductController()
        {
            _context = new FoodDbContext();   
        }
        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            var productViewModels = products.Select(MapToViewModel).ToList();
            return View(productViewModels);
        }


        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // Save the image file to the Images folder
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string imagePath = Server.MapPath("~/Images/");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(imagePath, uniqueFileName);
                    imageFile.SaveAs(filePath);

                    model.ImageFileName = uniqueFileName;
                }

                // Create a Product entity from the ViewModel
                Product newProduct = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    ImageFileName = model.ImageFileName
                };

                // Add the product to the database
                _context.Products.Add(newProduct);
                _context.SaveChanges();

                return RedirectToAction("Index"); // Redirect to the product list page
            }

            return View(model);
        }
        // Example mapping function
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

        // Controllers/ProductsController.cs
        public ActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return View();
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName
            };

            return View(viewModel);
        }
        // Controllers/ProductsController.cs

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = _context.Products.Find(viewModel.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found.");
                    return View(viewModel);
                }

                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.Price = viewModel.Price;

                // Update image in the application folder
                if (viewModel.ImageFile != null && viewModel.ImageFile.ContentLength > 0)
                {
                    string imagePath = Path.Combine(Server.MapPath("~/Images"), product.ImageFileName);

                    // Delete existing image
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    // Save new image
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(viewModel.ImageFile.FileName);
                    string newImagePath = Path.Combine(Server.MapPath("~/Images"), uniqueFileName);
                    viewModel.ImageFile.SaveAs(newImagePath);

                    // Update image filename in the database
                    product.ImageFileName = uniqueFileName;
                }

                try
                {
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception )
                {
                    

                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                }
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(viewModel);
        }


        public ActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return RedirectToAction("Index");
            }

            var viewModel = new ProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageFileName = product.ImageFileName
            };

            return View(viewModel);
        }
        // Controllers/ProductController.cs

        // Controllers/ProductController.cs


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return RedirectToAction("Index");
            }

           
            // Delete image from the image folder
            DeleteImage(product.ImageFileName);

            // Remove product from the database
            _context.Products.Remove(product);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }

      
        private void DeleteImage(string imageFileName)
        {
            if (!string.IsNullOrEmpty(imageFileName))
            {
                string imagePath = Path.Combine(Server.MapPath("~/Images"), imageFileName);

                // Delete image if it exists
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
        }
        [HttpPost]
        public ActionResult RemoveCartItem(int cartId)
        {
            // Ensure the user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "User not logged in.";
                return RedirectToAction("ViewCart");
            }

            // Retrieve the user's ID from the session
            int userId = Convert.ToInt32(Session["UserId"]);

            // Find the cart item for the specified cartId and userId
            var cartItem = _context.Carts.FirstOrDefault(c => c.CartId == cartId && c.CusomerId == userId);

            if (cartItem == null)
            {
                TempData["ErrorMessage"] = "Cart item not found.";
                return RedirectToAction("ViewCart", "Customer");
            }

            // Remove the cart item
            _context.Carts.Remove(cartItem);

            // Save changes and check for errors
            _context.SaveChanges();

            if (_context.ChangeTracker.HasChanges())
            {
                TempData["SuccessMessage"] = "Product removed from the cart successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while saving changes.";
            }

            return RedirectToAction("ViewCart","Customer");
        }

    }
}