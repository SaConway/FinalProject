namespace MsorLi.Utilities
{
    public class EncryptDecrypt
    {
        public static string Encrypt(string text)
        {
            string _result = string.Empty;
            char[] temp = text.ToCharArray();
            foreach (var _singleChar in temp)
            {
                var i = (int)_singleChar;
                i = i - 2;
                _result += (char)i;
            }
            return _result;
        }

        public static string Decrypt(string text)
        {
            string _result = string.Empty;
            char[] temp = text.ToCharArray();
            foreach (var _singleChar in temp)
            {
                var i = (int)_singleChar;
                i = i + 2;
                _result += (char)i;
            }
            return _result;
        }
    }
}