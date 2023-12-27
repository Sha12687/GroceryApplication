using FoodDeliveryDAL.Data;
using FoodDeliveryDAL.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDeliveryDAL.Repository
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly FoodDbContext _context;

        public OrderDetailRepository(FoodDbContext context)
        {
            _context = context;
        }

        public void CreateOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
        }

        public OrderDetail GetOrderDetailById(int orderDetailId)
        {
            return _context.OrderDetails.Find(orderDetailId);
        }

        public IEnumerable<OrderDetail> GetAllOrderDetails()
        {
            return _context.OrderDetails.ToList();
        }

        public void UpdateOrderDetail(OrderDetail orderDetail)
        {
            _context.Entry(orderDetail).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteOrderDetail(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
            _context.SaveChanges();
        }
    }

}
