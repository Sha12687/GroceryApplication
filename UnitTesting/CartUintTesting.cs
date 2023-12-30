using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Repository;
using Moq;
using NUnit.Framework;
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
        public void CreateCartItem_ShouldAddToDatabase()
        {

            var fakeObject = new Mock<ICartRepository>();
            fakeObject.Setup(x => x.CreateCartItem(It.IsAny<Cart>())).Returns(CartItem);
            var result = fakeObject.Object.CreateCartItem(CartItem);

            Assert.That(result, Is.EqualTo(CartItem));

        }

    }
}
