using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDeliveryDAL.Interface
{
    public interface IAddressRepository
    {
        IEnumerable<Address> GetAddressesByUserId(int userId);
        void SaveAddress(Address address);
        // Add other methods as needed
    }
}
