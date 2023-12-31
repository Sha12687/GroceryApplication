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
    public class AddressRepository : IAddressRepository
    {
        private readonly FoodDbContext _dbContext;

        public AddressRepository(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Address> GetAddressesByUserId(int userId)
        {
            return _dbContext.Addresses.Where(a => a.CustomerId == userId).ToList();
        }

        public void SaveAddress(Address address)
        {
            _dbContext.Addresses.Add(address);
            _dbContext.SaveChanges();
        }
    }
}
