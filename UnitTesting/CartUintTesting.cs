using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Repository;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Legacy;

namespace UnitTesting
{
    [TestFixture]
    public class CartUintTesting
    {

        public Cart CartItem;
        public Product ProductItem;
        public Customer CustomerItem;


        [SetUp]
        public void TestInitialize()
        {
       // Create instances of Product, Customer, and Cart for testing
            ProductItem = new Product
            {
                ProductQuantity = 10,
                ProductId = 1,
                Name = "Sample Product",
                Description = "This is a sample product.",
                Price = 29.99m,
                ImageFileName = "sample-product.jpg"
            };

            CustomerItem = new Customer
            {
                Id = 1,
                // Add other properties as needed
            };

            CartItem = new Cart
            {
                CartId = 12,
                Customer=CustomerItem,
                CusomerId = CustomerItem.Id,
                ProductName = ProductItem.Name,
                Quantity = 7,
                Price = 19,
                ImageFileName = ProductItem.ImageFileName,
                ProductId = ProductItem.ProductId,
                Product = ProductItem
            };
        }

        [Test]
        //[Ignore("This module pass all the test cases")]
        public void CreateCartItem_ShouldAddToDatabase()
        {

            var fakeObject = new Mock<ICartRepository>();
            fakeObject.Setup(x => x.CreateCartItem(It.IsAny<Cart>())).Returns(CartItem);
            var result = fakeObject.Object.CreateCartItem(CartItem);

            Assert.That(result, Is.EqualTo(CartItem));

        }
        [Test]
        public void GetCartItemById_ShouldReturnCartItem()
        {
            // Arrange
            var fakeObject = new Mock<ICartRepository>();
            fakeObject.Setup(x => x.GetCartItemById(It.IsAny<int>())).Returns(CartItem);

            var cartRepository = fakeObject.Object;

            // Act
            var result = cartRepository.GetCartItemById(1);

            // Assert
            Assert.That(result, Is.EqualTo(CartItem));
        }


        [Test]
        //[Ignore("Passed")]
        public void RemoveCartItem_ShouldRemoveFromDatabase()
        {
            // Arrang
            var fakeObject = new Mock<ICartRepository>();
            fakeObject.Setup(x => x.RemoveCartItem(It.IsAny<Cart>())).Returns(CartItem);

            var cartRepository = fakeObject.Object;
            // Act
            var result = cartRepository.RemoveCartItem(CartItem);
            //note that this test is not actually interacting with a real database. It's checking how the method behaves based on the setup provided by Moq.
            Assert.That(result.CartId, Is.EqualTo(CartItem.CartId));
        }
    }
}
