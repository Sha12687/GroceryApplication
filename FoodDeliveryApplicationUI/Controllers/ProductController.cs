﻿using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Interface;
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
    [Authorize(Roles="Admin")]
    public class ProductController : Controller
    {
       
     //   private readonly FoodDbContext  _context;
        private readonly IProductRepository productRepository;
        private readonly ICartRepository cartRepository;

        public ProductController(IProductRepository productRepository,ICartRepository cartRepository)
        {
          
            this.productRepository = productRepository;
            this.cartRepository = cartRepository;
        }
        public ActionResult Index()
        {
            var products = productRepository.GetAllProducts();
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
                    ImageFileName = model.ImageFileName,
                    ProductQuantity = model.ProductQuantity,
                };

                // Add the product to the database
                productRepository.CreateProduct(newProduct);
             

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
                ImageFileName = product.ImageFileName,
                ProductQuantity= product.ProductQuantity,
                // Add other properties as needed
            };
        }

        // Controllers/ProductsController.cs
        public ActionResult Edit(int id)
        {
            var product = productRepository.GetProductById(id);
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
                ImageFileName = product.ImageFileName,
                ProductQuantity= product.ProductQuantity,
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
                var product = productRepository.GetProductById(viewModel.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found.");
                    return View(viewModel);
                }

             product.Name = viewModel.Name;
             product.Description = viewModel.Description;
             product.Price = viewModel.Price;
             product.ProductQuantity = viewModel.ProductQuantity;

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

                    product.ImageFileName = uniqueFileName;
                }

                try
                {
                    productRepository.SaveProductChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception )
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while processing your request. Please try again later.");
                }
            }

            return View(viewModel);
        }


        public ActionResult Delete(int id)
        {
            var product = productRepository.GetProductById(id);
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
                ImageFileName = product.ImageFileName,
                ProductQuantity = product.ProductQuantity,
            };

            return View(viewModel);
        }
        // Controllers/ProductController.cs


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = productRepository.GetProductById(id);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "Product not found.");
                return RedirectToAction("Index");
            }

           
            // Delete image from the image folder
            DeleteImage(product.ImageFileName);

            // Remove product from the database
           productRepository.DeleteProduct(product.ProductId);
           

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
     

    }
}