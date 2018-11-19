using System;
using System.Text;

namespace Public.Core.Security
{
    /// <summary>
    ///   V1.0
    /// </summary>
    public static class SHAHelper
    {
        /// <summary>
        ///SHA256位加密 不可逆
        /// </summary>
        /// <param name="plainText">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SHA256Encrypt(string plainText)
        {
            string salt = "264e0d09-264f-4ce9-9e59-5012c6302b8e";
            var SHA = System.Security.Cryptography.SHA256.Create();
            byte[] passwordAndSaltBytes = Encoding.Unicode.GetBytes(plainText + salt.ToUpper());
            byte[] hashBytes = SHA.ComputeHash(passwordAndSaltBytes);
            string hashString = Convert.ToBase64String(hashBytes);
            return hashString;
        }
    }
}
