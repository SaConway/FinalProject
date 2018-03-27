using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
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

        public async Task<ObservableCollection<User>> LoginAsync(string email , string password)
        {
            try
            {


                //IEnumerable<User> user1 = await _table.Where(filter);
                //return user;
                IEnumerable<User> user_task = await _table.Where(user => user.Email == email && user.Password == password).ToEnumerableAsync();
                return new ObservableCollection<User>(user_task);
            }

            catch (Exception) { }
            return null;
        }
    }
}

