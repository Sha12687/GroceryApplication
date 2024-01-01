using FoodDeliveryApplicationUI.Controllers;
using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    public class HomeControllerTesting
    {


        [Test]
        public void MapToViewModel_Should_Map_Customer_To_CustomerModel()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                FirstName = "sivasai",
                LastName = "jayanthi",
                Email = "sai@gmail.com",
                PhoneNumber = "1234567890",
                UserName = "sai",
                Password = "password"
            };

            var homeController = new HomeController();

            // Act
            var result = homeController.MapToViewModel(customer);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsInstanceOf<CustomerModel>(result);
            ClassicAssert.AreEqual(customer.Id, result.Id);
            ClassicAssert.AreEqual(customer.FirstName, result.FirstName);
            ClassicAssert.AreEqual(customer.LastName, result.LastName);
            ClassicAssert.AreEqual(customer.Email, result.Email);
            ClassicAssert.AreEqual(customer.PhoneNumber, result.PhoneNumber);
            ClassicAssert.AreEqual(customer.UserName, result.UserName);
            ClassicAssert.AreEqual(customer.Password, result.Password);

        }

    }
}
