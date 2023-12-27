using FoodDeliveryDAL.Interface;
using FoodDeliveryDAL.Repository;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace FoodDeliveryApplicationUI
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<IAdminRepository, AdminRepository>();
            container.RegisterType<ICartRepository, CartRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IOrderDetailRepository, OrderDetailRepository>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}