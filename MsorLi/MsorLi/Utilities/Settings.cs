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

        private const string _SettingsKey = "settings_key";
        private static readonly string _SettingsDefault = string.Empty;

        #endregion

        public static string _GeneralSettings
        {
            get
            {
                return _AppSettings.GetValueOrDefault(_SettingsKey, _SettingsDefault, "Settings.txt");
            }
            set
            {
                _AppSettings.AddOrUpdateValue(_SettingsKey, value, "Settings.txt");
            }
        }
    }
}