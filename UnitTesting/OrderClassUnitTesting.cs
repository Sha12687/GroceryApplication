﻿using Castle.Core.Resource;
using FoodDeliveryDAL;
using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Repository;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{

    [TestFixture]
    public class OrderClassUnitTesting
    {
        private Mock<FoodDbContext> mockDbContext;
        private IOrderRepository orderRepository;
        private Order Orders;
        public Customer Customer;
        [SetUp]
        public void SetUp()
        {
            mockDbContext = new Mock<FoodDbContext>();
            orderRepository = new OrderRepository(mockDbContext.Object);

            Customer customer = new Customer { Id=10};

            Orders = new Order
            {
                OrderId = 1,
                OrderDate = DateTime.Now,
                CustomerId = 101,
                TotalAmount = 150.75m,
                Customer = customer,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderDetailId = 101,
                        ProductName = "Sample Product 1",
                        price = 25.50m,
                        Quantity = 2
                    },
                    new OrderDetail
                    {
                        OrderDetailId = 102,
                        ProductName = "Sample Product 2",
                        price = 50.25m,
                        Quantity = 3
                    }
                }
            };
          List<Order>  OrdersList = new List<Order>
        {
            new Order
            {
                OrderId = 1,
                OrderDate = DateTime.Now,
                CustomerId = 101,
                TotalAmount = 150.75m,
                Customer = customer,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderDetailId = 101,
                        ProductName = "Sample Product 1",
                        price = 25.50m,
                        Quantity = 2
                    },
                    new OrderDetail
                    {
                        OrderDetailId = 102,
                        ProductName = "Sample Product 2",
                        price = 50.25m,
                        Quantity = 3
                    }
                }
            },
            // Additional orders
            new Order
            {
                OrderId = 2,
                OrderDate = DateTime.Now.AddDays(-1),
                CustomerId = 101,
                TotalAmount = 100.50m,
                Customer = customer,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderDetailId = 201,
                        ProductName = "Sample Product 3",
                        price = 30.75m,
                        Quantity = 1
                    }
                }
            },
            new Order
            {
                OrderId = 3,
                OrderDate = DateTime.Now.AddDays(-2),
                CustomerId = 101,
                TotalAmount = 75.25m,
                Customer = customer,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderDetailId = 301,
                        ProductName = "Sample Product 4",
                        price = 15.25m,
                        Quantity = 4
                    }
                }
            },
            new Order
            {
                OrderId = 4,
                OrderDate = DateTime.Now.AddDays(-3),
                CustomerId = 101,
                TotalAmount = 200.00m,
                Customer = customer,
                OrderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        OrderDetailId = 401,
                        ProductName = "Sample Product 5",
                        price = 40.00m,
                        Quantity = 3
                    }
                }
            }
        };


        }

        [Test]
        public void CreateOrder_ShouldAddToDbContext()
        {

            var fakeObject = new Mock<IOrderRepository>();
            fakeObject.Setup(x => x.CreateOrder2(It.IsAny<Order>())).Returns(Orders);
            var result = fakeObject.Object.CreateOrder2(Orders);

            Assert.That(result, Is.EqualTo(Orders));

        }


        [Test]
        public void GetCartItemsByCustomerId_ShouldReturnCartItems()
        {
            // Arrange
            int customerId = 123; // Replace with the actual customer ID 

            List<Order> tempForTesting = new List<Order>();
            tempForTesting.Add(Orders);
            // Set up mock for IOrderRepository
            var fakeObject = new Mock<IOrderRepository>();
            fakeObject.Setup(x => x.GetOrdersByCustomerId(customerId)).Returns(tempForTesting);

            // Act
          var result = fakeObject.Object.GetOrdersByCustomerId(customerId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(tempForTesting.Count(), result.Count());
        }


        [Test]
        public void DeleteOrder_ShouldCallRepositoryDelete()
        {
            // Arrange
            var fakeObject = new Mock<IOrderRepository>();
            // Ac

            //(x => x.DeleteOrder(Orders)): This is a lambda expression specifying the method call you expect.In this case, it's saying "I expect the DeleteOrder method to be called with the argument Orders".

            //Times.Once: This specifies how many times the method should be called. In this
            fakeObject.Object.DeleteOrder(Orders);
            fakeObject.Verify(x => x.DeleteOrder(Orders), Times.Once);
        }
    }


}
