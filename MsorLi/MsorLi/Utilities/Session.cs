using System;
namespace MsorLi.Utilities
{
    public static class Session
    {
        public static bool IsLogged()
        {
            return (string.IsNullOrEmpty(Settings.UserId)) ? false : true;
        }

    }
}
