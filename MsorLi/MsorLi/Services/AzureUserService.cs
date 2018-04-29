using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MsorLi.Models;
using MsorLi.Utilities;

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
            {}
            return false;
        }

        public async Task<User> IsFacebookIdExistAsync(string facebookId)
        {
            try
            {
                var user = await _table
                    .Where(User => User.FacebookId == facebookId)
                    .ToListAsync();

                return user.Count != 0 ? user[0] : null;
            }

            catch (Exception)
            {}
            return null;
        }

        public async Task<int> UpdateNumOfItems(string userId, int prefix)
        {
            try
            {
                var user = await _table
                    .LookupAsync(userId);

                user.NumOfItems += prefix;

                await UploadToServer(user, user.Id);

                return user.NumOfItems;

            }
            catch(Exception)
            {
                return Int32.Parse(Settings.NumOfItems);
            }
        }

        public async Task<int> UpdateNumOfItemsLiked(string userId, int prefix)
        {
            try
            {
                var user = await _table
                    .LookupAsync(userId);

                user.NumOfItemsUserLike += prefix;

                await UploadToServer(user, user.Id);

                return user.NumOfItemsUserLike;

            }
            catch (Exception)
            {
                return Int32.Parse(Settings.NumOfItemsUserLike);
            }
        }

        public async Task<int> getNumOfItems(string userId)
        {
            try
            {
                var user = await _table
                    .LookupAsync(userId);

                return user.NumOfItems;

            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<User> UpdateUser(User newUser)
        {
            try
            {
                await UploadToServer(newUser, newUser.Id);

                var user = await _table
                    .LookupAsync(newUser.Id);

                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}