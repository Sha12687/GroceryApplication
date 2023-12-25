using FoodDeliveryDAL.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace FoodDeliveryDAL.Service
{
    public static class Authentication
    {
        private  static readonly FoodDbContext context = new FoodDbContext();

       
        public static bool VerifyAdminCredentials(string username, string password)
        {
            // Your logic to check username and password in the database
            var admin = context.Admins.FirstOrDefault(a => a.UserName == username);

            if (admin != null)
            {
                var passwordHasher = new PasswordHasher<Admin>();
                var result = passwordHasher.VerifyHashedPassword(admin, admin.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    return true; // Username and password are correct
                }
            }

            return false; // Invalid username or password
        }
        public static bool VerifyCustomerCredentials(string username, string password)
        {
            // Your logic to check username and password in the database
            var cutomer = context.Customers.FirstOrDefault(a => a.UserName == username);

            if (cutomer != null)
            {
                var passwordHasher = new PasswordHasher<Customer>();
                var result = passwordHasher.VerifyHashedPassword(cutomer, cutomer.Password, password);

                if (result == PasswordVerificationResult.Success)
                {
                    return true; // Username and password are correct
                }
            }

            return false; // Invalid username or password
        }
    }
}
