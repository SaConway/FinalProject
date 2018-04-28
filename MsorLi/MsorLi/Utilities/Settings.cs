using MsorLi.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MsorLi.Utilities
{
    public static class Settings
    {
        private static ISettings _AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Settings Constants

        private static readonly string _SettingsDefault = string.Empty;

        #endregion

        public static string UserId
        {
            get => _AppSettings.GetValueOrDefault(nameof(UserId), _SettingsDefault, "Settings.txt");
            set =>_AppSettings.AddOrUpdateValue(nameof(UserId), value, "Settings.txt");
        }

        public static string UserFirstName
        {
            get =>  _AppSettings.GetValueOrDefault(nameof(UserFirstName), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(UserFirstName), value, "Settings.txt");
        }

        public static string UserLastName
        {
            get => _AppSettings.GetValueOrDefault(nameof(UserLastName), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(UserLastName), value, "Settings.txt");
        }

        public static string ImgUrl
        {
            get => _AppSettings.GetValueOrDefault(nameof(ImgUrl), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(ImgUrl), value, "Settings.txt");
        }

        public static string Email
        {
            get => _AppSettings.GetValueOrDefault(nameof(Email), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(Email), value, "Settings.txt");
        }

        public static string Phone
        {
            get => _AppSettings.GetValueOrDefault(nameof(Phone), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(Phone), value, "Settings.txt");
        }

        public static string Address
        {
            get => _AppSettings.GetValueOrDefault(nameof(Address), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(Address), value, "Settings.txt");
        }

        public static string Permission
        {
            get => _AppSettings.GetValueOrDefault(nameof(Permission), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(Permission), value, "Settings.txt");
        }
        public static string NumOfItems
        {
            get => _AppSettings.GetValueOrDefault(nameof(NumOfItems), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(NumOfItems), value, "Settings.txt");
        }
        public static string NumOfItemsUserLike
        {
            get => _AppSettings.GetValueOrDefault(nameof(NumOfItemsUserLike), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(NumOfItemsUserLike), value, "Settings.txt");
        }
        public static string Password
        {
            get => _AppSettings.GetValueOrDefault(nameof(Password), _SettingsDefault, "Settings.txt");
            set => _AppSettings.AddOrUpdateValue(nameof(Password), value, "Settings.txt");
        }

        public static void ClearUserData(){
            _AppSettings.Clear("Settings.txt");
        }

        public static void UpdateUserInfo(User user)
        {
            Settings.UserId = user.Id;
            Settings.UserFirstName = user.FirstName;
            Settings.UserLastName = user.LastName;
            Settings.ImgUrl = user.ImgUrl;
            Settings.Email = user.Email;
            Settings.Phone = user.Phone;
            Settings.Address = user.Address;
            Settings.Permission = user.Permission;
            Settings.NumOfItems = user.NumOfItems.ToString();
            Settings.NumOfItemsUserLike = user.NumOfItemsUserLike.ToString();
            Settings.Password = user.Password;
        }
    }
}