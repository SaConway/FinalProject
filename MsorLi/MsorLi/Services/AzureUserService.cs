using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MsorLi.Models;

namespace MsorLi.Services
{
    public class AzureUserService :AzureService<User>
    {
        //---------------------------------
        // MEMBERS
        //---------------------------------

        static AzureUserService _defaultInstance = new AzureUserService();

        public static AzureUserService DefaultManager
        {
            get
            {
                return _defaultInstance;
            }
            private set
            {
                _defaultInstance = value;
            }
        }

        //---------------------------------
        // FUNCTIONS
        //---------------------------------

        public async Task<User> GetUserAsync(string email ,string password)
        {
            try
            {
                var user = await _table
                    .Where(User => User.Email == email)
                    .ToListAsync();

                return user.Count != 0 ? user[0] : null;
            }

            catch (Exception) { }
            return null;
        }

        public async Task<bool> IsEmailExistAsync(string email)
        {
            try
            {
                var user = await _table
                    .Where(User => User.Email == email)
                    .ToListAsync();

                return user.Count != 0 ? true : false;
            }

            catch (Exception)
            {

            }
            return false;
        }
    }
}