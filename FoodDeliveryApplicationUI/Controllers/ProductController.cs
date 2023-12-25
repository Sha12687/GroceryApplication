using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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

    }
}