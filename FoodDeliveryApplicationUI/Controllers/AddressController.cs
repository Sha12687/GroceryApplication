﻿using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodDeliveryApplicationUI.Controllers
{
    [Authorize (Roles= "Customer")]
    public class AddressController : Controller
    {
        private IAddressRepository AddressRepository;
        private readonly ICartRepository cartRepository;

        // GET: Address
        public AddressController(IAddressRepository addressRepository
            ,ICartRepository cartRepository)
        {
            AddressRepository = addressRepository;
            this.cartRepository = cartRepository;
        }
        public ActionResult Address()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAddress(AddressViewModel address)
        {
            int customerId = (int)Session["UserId"];
            Address saverAddress = new Address
            {
                Id = address.Id,
                State = address.State,
                Street = address.Street,
                City = address.City,
                PostalCode = address.PostalCode,
                Country = address.Country,
                CustomerId= customerId
            };

            // After successfully adding the address, you can set a TempData flag
            AddressRepository.SaveAddress(saverAddress);
            TempData["AddressAdded"] = true;
            return RedirectToAction("PlaceOrder", "Customer", new { cartIds= Convert.ToInt32(null), customerId = customerId });
        }
    }
}