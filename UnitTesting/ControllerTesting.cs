using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using FoodDeliveryDAL;
using FoodDeliveryApplicationUI.Controllers;
using NUnit.Framework.Legacy;
using FoodDeliveryApplicationUI.Models;
using FoodDeliveryDAL.Interface;
using System.Web.Mvc;
using Moq;
using FoodDeliveryDAL.Repository;
using FoodDeliveryDAL.Data;
using System.Web;
using System.IO;



namespace test
{
    [TestFixture]
    public class ControllerTests
    {

        //account controller tests
        //[Test]
        //public void ResetPassword_ValidModel_RedirectsToCustomerLogin()
        //{
        //    // Arrange
        //    var adminRepositoryMock = new Mock<IAdminRepository>();
        //    var customerRepositoryMock = new Mock<ICustomerRepository>();
        //    var controller = new AccountController(adminRepositoryMock.Object, customerRepositoryMock.Object);
        //    var resetPasswordViewModel = new ResetPasswordViewModel
        //    {
        //        UserName = "customer",
        //        PhoneNumber = "8896477007"
        //    };
         
        //    customerRepositoryMock.Setup(repo => repo.GetCustomerByUserName(It.IsAny<string>()))
        //        .Returns(new Customer { Id = 1, UserName = "customer" }); // Adjust as needed

        //    // Act
        //    var result = controller.ResetPassword(resetPasswordViewModel) as RedirectToRouteResult;

        //    // Assert
        //    ClassicAssert.IsNotNull(result);
        //    ClassicAssert.AreEqual("CustomerLogin", result.RouteValues["action"]);
        //    ClassicAssert.AreEqual("Account", result.RouteValues["controller"]);
        //}



        [Test]
        public void Registration_ValidCustomer_RedirectsToCustomerIndex()
        {
            // Arrange
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var controller = new AccountController(adminRepositoryMock.Object, customerRepositoryMock.Object);
            var userView = new UserView
            {
                Email = "newcustomer@gmail.com",
                UserName = "newcustomer",
                FirstName = "ramu",
                LastName = "y",
                PhoneNumber = "1234567890",
                UserType = 2,
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = controller.Registration(userView) as RedirectToRouteResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);
            ClassicAssert.AreEqual("Customer", result.RouteValues["controller"]);
        }

        [Test]
        public void Registration_ValidAdmin_RedirectsToAdminIndex()
        {
            // Arrange
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var controller = new AccountController(adminRepositoryMock.Object, customerRepositoryMock.Object);
            var userView = new UserView
            {
                Email = "newadmin@gmail.com",
                UserName = "newadmin",
                FirstName = "Admin",
                LastName = "User",
                PhoneNumber = "1234567890",
                UserType = 1,
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            // Act
            var result = controller.Registration(userView) as RedirectToRouteResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);
            ClassicAssert.AreEqual("Admin", result.RouteValues["controller"]);
        }

        [Test]
        public void Registration_ExistingEmail_ReturnsViewWithModelError()
        {
            // Arrange
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var controller = new AccountController(adminRepositoryMock.Object, customerRepositoryMock.Object);
            var existingEmail = "existingemail@gmail.com";
            var userView = new UserView
            {
                Email = existingEmail,
                UserName = "newuser",
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            customerRepositoryMock.Setup(repo => repo.CustomerExistsEmail(existingEmail)).Returns(true);

            // Act
            var result = controller.Registration(userView) as ViewResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Registration", result.ViewName);
            ClassicAssert.IsTrue(result.ViewData.ModelState.ContainsKey("Email"));
            ClassicAssert.AreEqual("Email already registered with us.", result.ViewData.ModelState["Email"].Errors[0].ErrorMessage);
        }

        [Test]
        public void Registration_ExistingUserName_ReturnsViewWithModelError()
        {
            // Arrange
            var adminRepositoryMock = new Mock<IAdminRepository>();
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var controller = new AccountController(adminRepositoryMock.Object, customerRepositoryMock.Object);
            var existingUserName = "existinguser";
            var userView = new UserView
            {
                Email = "newemail@example.com",
                UserName = existingUserName,
                Password = "newpassword",
                ConfirmPassword = "newpassword"
            };

            customerRepositoryMock.Setup(repo => repo.CustomerExists(existingUserName)).Returns(true);

            // Act
            var result = controller.Registration(userView) as ViewResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Registration", result.ViewName);
            ClassicAssert.IsTrue(result.ViewData.ModelState.ContainsKey("UserName"));
            ClassicAssert.AreEqual("Username already registered with us.", result.ViewData.ModelState["UserName"].Errors[0].ErrorMessage);
        }


        // product controller tests


        [Test]
        public void Edit_ValidProductId_ReturnsViewWithProductDetails()
        {
            // Arrange
            int productId = 1;
            var productRepositoryMock = new Mock<IProductRepository>();
            var cartRepositoryMock = new Mock<ICartRepository>();

            // Assuming you have a utility method to map a Product to ProductViewModel
            var productViewModel = new ProductViewModel { };
            productRepositoryMock.Setup(repo => repo.GetProductById(productId))
                .Returns(new Product { ProductId = productId, });

            var controller = new ProductController(productRepositoryMock.Object, cartRepositoryMock.Object);

            // Act
            var result = controller.Edit(productId) as ViewResult;
            var model = result?.Model as ProductViewModel;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.IsNotNull(model);

        }


        [Test]
        public void DeleteConfirmed_ValidProductId_DeletesProductAndRedirectsToIndex()
        {
            // Arrange
            int productId = 1; // Replace with a valid product ID
            var productRepositoryMock = new Mock<IProductRepository>();
            var cartRepositoryMock = new Mock<ICartRepository>();

            var controller = new ProductController(productRepositoryMock.Object, cartRepositoryMock.Object);

            // Assuming you have a utility method to map a Product to ProductViewModel
            var productViewModel = new ProductViewModel { };
            productRepositoryMock.Setup(repo => repo.GetProductById(productId))
                .Returns(new Product { ProductId = productId, });

            // Act
            var result = controller.DeleteConfirmed(productId) as RedirectToRouteResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Index", result.RouteValues["action"]);

            // Verify that the DeleteProduct method was called
            productRepositoryMock.Verify(repo => repo.DeleteProduct(productId), Times.Once);
        }









        //admin controller tests

        [Test]
        public void EditAdmin_WithValidUserView_RedirectsToViewProfile()
        {
            // Arrange
            int adminId = 1; // Replace with a valid admin ID
            var adminRepositoryMock = new Mock<IAdminRepository>();

            var controller = new AdminController(null, null, adminRepositoryMock.Object);

            var userView = new UserView
            {
                Id = adminId,
                FirstName = "ravi",
                LastName = "b",
                Email = "ravi@gmail.com",
                PhoneNumber = "1234567890",
                UserName = "ravi"

            };

            adminRepositoryMock.Setup(repo => repo.GetAdminById(adminId)).Returns(new Admin { Id = adminId, });

            // Act
            var result = controller.EditAdmin(userView) as RedirectToRouteResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("ViewProfile", result.RouteValues["action"]);
            ClassicAssert.AreEqual("Admin", result.RouteValues["controller"]);
            ClassicAssert.AreEqual(adminId, result.RouteValues["AdminId"]);

            // Verify that SaveAdminChanges was called
            adminRepositoryMock.Verify(repo => repo.SaveAdminchages(), Times.Once);
        }


        [Test]
        public void GetAdminProfile_ValidAdminId_ReturnsUserView()
        {
            // Arrange
            int adminId = 1; // Replace with a valid admin ID
            var adminRepositoryMock = new Mock<IAdminRepository>();

            var controller = new AdminController(null, null, adminRepositoryMock.Object);

            adminRepositoryMock.Setup(repo => repo.GetAdminById(adminId)).Returns(new Admin
            {
                Id = adminId,
                FirstName = "tina",
                LastName = "p",
                UserName = "tina",
                Email = "tina@gmail.com",
                PhoneNumber = "1234567890"

            });

            // Act
            var result = controller.GetAdminProfile(adminId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(adminId, result.Id);
            ClassicAssert.AreEqual("tina", result.FirstName);
            ClassicAssert.AreEqual("p", result.LastName);
            ClassicAssert.AreEqual("tina", result.UserName);
            ClassicAssert.AreEqual("tina@gmail.com", result.Email);
            ClassicAssert.AreEqual("1234567890", result.PhoneNumber);

            // Verify that GetAdminById was called
            adminRepositoryMock.Verify(repo => repo.GetAdminById(adminId), Times.Once);
        }
    }
}












// customer controller



//[Test]
//public void EditCustomer_ValidCustomerId_ReturnsViewWithUpdatedCustomerProfile()
//{
//    // Arrange
//    int customerId = 1;
//    var customerRepositoryMock = new Mock<ICustomerRepository>();
//    customerRepositoryMock.Setup(repo => repo.GetCustomerById(customerId))
//        .Returns(new Customer { Id = customerId, UserName = "JohnDoe" });
//    var controller = new CustomerController(customerRepositoryMock.Object, null, null, null, null);
//    var updatedCustomer = new CustomerModel { Id = customerId, UserName = "UpdatedJohnDoe" };

//    // Act
//    var result = controller.EditCustomer(updatedCustomer) as RedirectToRouteResult;

//    // Assert
//    ClassicAssert.IsNotNull(result);
//    ClassicAssert.AreEqual("ViewProfile", result.RouteValues["action"]);
//    ClassicAssert.AreEqual(customerId, result.RouteValues["customerId"]);

//    customerRepositoryMock.Verify(repo => repo.GetCustomerById(customerId), Times.Once);
//    customerRepositoryMock.Verify(repo => repo.customerSAveChanges(), Times.Once);
//}




//[Test]
//public void UpdateCartQuantity_WithValidCartIdAndQuantity_UpdatesCartAndReturnsView()
//{
//    // Arrange
//    int cartId = 1; // Replace with a valid cart ID
//    int newQuantity = 3;
//    var cartRepositoryMock = new Mock<ICartRepository>();

//    var controller = new CustomerController(
//        Mock.Of<ICustomerRepository>(),
//        Mock.Of<IProductRepository>(),
//        cartRepositoryMock.Object,
//        Mock.Of<IOrderRepository>(),
//        Mock.Of<IOrderDetailRepository>()
//    );

//    // Act
//    var result = controller.UpdateCartQuantity(cartId, newQuantity) as ViewResult;

//    // Assert
//    ClassicAssert.IsNotNull(result);
//    ClassicAssert.AreEqual(string.Empty, result.ViewName); // Assuming an empty string for the view name
//}




//[Test]
//public void EditCustomer_WithValidCustomerModel_RedirectsToViewProfile()
//{
//    // Arrange
//    int customerId = 1; // Replace with a valid customer ID
//    var customerRepositoryMock = new Mock<ICustomerRepository>();
//    var controller = new CustomerController(customerRepositoryMock.Object, Mock.Of<IProductRepository>(), Mock.Of<ICartRepository>(), Mock.Of<IOrderRepository>(), Mock.Of<IOrderDetailRepository>());

//    var validCustomerModel = new CustomerModel
//    {
//        Id = customerId,
//        FirstName = "Updated",
//        LastName = "User",
//        Email = "updated.user@example.com",
//        PhoneNumber = "987-654-3210",
//        UserName = "updated.user"
//    };

//    var existingCustomer = new Customer
//    {
//        Id = customerId,
//        FirstName = "siva",
//        LastName = "sai",
//        Email = "siva@example.com",
//        PhoneNumber = "123-456-7890",
//        UserName = "sivasai"
//    };

//    customerRepositoryMock.Setup(repo => repo.GetCustomerById(customerId)).Returns(existingCustomer);

//    // Act
//    var result = controller.EditCustomer(validCustomerModel) as RedirectToRouteResult;

//    // Assert
//    ClassicAssert.IsNotNull(result);
//    ClassicAssert.AreEqual("ViewProfile", result.RouteValues["action"]);
//    ClassicAssert.AreEqual("Customer", result.RouteValues["controller"]);
//    // Add more assertions as needed

//    // Verify that the repository method was called
//    customerRepositoryMock.Verify(repo => repo.GetCustomerById(customerId), Times.Once);
//    customerRepositoryMock.Verify(repo => repo.customerSAveChanges(), Times.Once);
//    // Add more verification as needed
//}


//[Test]
//public void GetCustomerProfile_ValidCustomerId_ReturnsCustomerModel()
//{
//    // Arrange
//    int customerId = 1; // Replace with a valid customer ID
//    var customerRepositoryMock = new Mock<ICustomerRepository>();
//    var controller = new CustomerController(customerRepositoryMock.Object, Mock.Of<IProductRepository>(), Mock.Of<ICartRepository>(), Mock.Of<IOrderRepository>(),Mock.Of<IAddressRepository>);

//    var existingCustomer = new Customer
//    {
//        Id = customerId,
//        FirstName = "june",
//        LastName = "w",
//        UserName = "june",
//        Email = "june@gmail.com",
//        PhoneNumber = "123-456-7890"
//    };

//    customerRepositoryMock.Setup(repo => repo.GetCustomerById(customerId)).Returns(existingCustomer);

//    // Act
//    var result = controller.GetCustomerProfile(customerId);

//    // Assert
//    ClassicAssert.IsNotNull(result);
//    ClassicAssert.AreEqual(customerId, result.Id);
//    ClassicAssert.AreEqual(existingCustomer.FirstName, result.FirstName);
//    ClassicAssert.AreEqual(existingCustomer.LastName, result.LastName);
//    ClassicAssert.AreEqual(existingCustomer.UserName, result.UserName);
//    ClassicAssert.AreEqual(existingCustomer.Email, result.Email);
//    ClassicAssert.AreEqual(existingCustomer.PhoneNumber, result.PhoneNumber);

//    // Verify that the repository method was called
//    customerRepositoryMock.Verify(repo => repo.GetCustomerById(customerId), Times.Once);
//}

