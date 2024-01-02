using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDeliveryDAL.Interface
{
    public interface IAdminRepository
    {
        // Create
        Admin CreateAdmin(Admin admin);

        // Read
        bool AdminExistsEmail(string UserEmail);
        bool AdminExists(string userName);
        Admin GetAdminById(int adminId);
        IEnumerable<Admin> GetAllAdmins();

        // Update
        Admin UpdateAdmin(Admin admin);

        // Delete
        bool AdminExistsEmail(string UserEmail, int id);
        Admin DeleteAdmin(int adminId);
        Admin GetAdminByUserName(string userName);
        void SaveAdminchages();
        Admin GetAdminByUserNamePhone(string userName, string phoneNumber);
    }

}
