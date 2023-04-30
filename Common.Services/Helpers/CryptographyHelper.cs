using System;
using System.Net;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Common.Services.Helpers
{
    public class CryptographyHelper
    {
        private const int ENCRYPTION_LENGTH = 195;

        private static byte[] _entropy = Encoding.Unicode.GetBytes("Entropy!23");
        public static string Entropy { set => _entropy = Encoding.Unicode.GetBytes(value); }

        public static NetworkCredential Decrypt(NetworkCredential networkCredential)
        {
            if (networkCredential is NetworkCredential credential)
            {
                networkCredential.UserName = DecryptString(credential.UserName);
                networkCredential.Password = DecryptString(credential.Password);
            }
            return networkCredential;
        }

        [SupportedOSPlatform("windows")]
        public static string EncryptString(string unencrypted)
        {
            string encrypted = "";

            if (unencrypted.Length > ENCRYPTION_LENGTH)
            {
                encrypted = unencrypted;
            }
            else if (!string.IsNullOrWhiteSpace(unencrypted))
            {
                var secureString = ToSecureString(unencrypted);
                encrypted = EncryptStringSecure(secureString);
            }

            return encrypted;
        }

        //[SupportedOSPlatform("windows")]
        public static string DecryptString(string encrypted)
        {
            string decrypted = "";

            if (encrypted.Length < ENCRYPTION_LENGTH)
            {
                decrypted = encrypted;
            }
            else if (!string.IsNullOrWhiteSpace(encrypted) && OperatingSystem.IsWindows())
            {
                var secureString = DecryptStringSecure(encrypted);
                decrypted = ToInsecureString(secureString);
            }
            return decrypted;
        }

        [SupportedOSPlatform("windows")]
        public static string EncryptStringSecure(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(
                Encoding.Unicode.GetBytes(ToInsecureString(input)),
                _entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        [SupportedOSPlatform("windows")]
        public static SecureString DecryptStringSecure(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    _entropy, DataProtectionScope.CurrentUser);
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        [SupportedOSPlatform("windows")]
        public static string Encrypt(string unencryptedData)
        {
            try
            {
                //if (OperatingSystem.IsWindows() &&
                //  !string.IsNullOrWhiteSpace(unencryptedData) &&
                //  unencryptedData.Length < ENCRYPTION_LENGTH)
                byte[] encryptedData = ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(unencryptedData), _entropy,
                    DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encryptedData);
            }
            catch
            {
            }
            return "";
        }

        [SupportedOSPlatform("windows")]
        public static string Decrypt(string encryptedData)
        {
            try
            {
                //if (OperatingSystem.IsWindows() &&
                //  !string.IsNullOrWhiteSpace(encryptedData) &&
                //  encryptedData.Length > ENCRYPTION_LENGTH)
                byte[] decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData), _entropy,
                    DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(decryptedData);
            }
            catch
            {
            }
            return "";
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
    }
}
