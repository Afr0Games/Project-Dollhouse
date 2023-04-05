using System.Security.Cryptography;

namespace GonzoNet.Encryption
{
    using System;
    using System.Text;
    using System.Security.Cryptography;

    /// <summary>
    /// AES class is derived from the MSDN .NET CreateEncryptor() example
    /// http://msdn.microsoft.com/en-us/library/09d0kyb3.aspx
    /// </summary>
    public class AES
    {
        // Symmetric algorithm interface is used to store the AES service provider
        private SymmetricAlgorithm AESProvider;

        // Crytographic transformers are used to encrypt and decrypt byte arrays
        private ICryptoTransform encryptor;
        private ICryptoTransform decryptor;

        /// <summary>
        /// Constructor for AES class that takes byte arrays for the key and IV
        /// </summary>
        /// <param name="key">Cryptographic key</param>
        /// <param name="IV">Cryptographic initialization vector</param>
        public AES(string Password, byte[] Salt)
        {
            using (Rfc2898DeriveBytes DeriveBytes = new Rfc2898DeriveBytes(Password, Salt))
            {
                // Initialize AESProvider with AES service provider
                AESProvider = new AesCryptoServiceProvider();
                AESProvider.Mode = CipherMode.CBC;

                byte[] Key = DeriveBytes.GetBytes(32); // AES-256 key
                byte[] IV = DeriveBytes.GetBytes(16); // AES block size is 128 bits (16 bytes)

                // Set the key and IV for AESProvider
                AESProvider.Key = Key;
                AESProvider.IV = IV;

                // Initialize cryptographic transformers from AESProvider
                encryptor = AESProvider.CreateEncryptor();
                decryptor = AESProvider.CreateDecryptor();
            }
        }

        /// <summary>
        /// Encrypts a string with AES
        /// </summary>
        /// <param name="plainText">String to encrypt</param>
        /// <returns>Encrypted string</returns>
        public string Encrypt(string PlainText)
        {
            byte[] PlainBytes = Encoding.UTF8.GetBytes(PlainText);
            byte[] SecureBytes = encryptor.TransformFinalBlock(PlainBytes, 0, PlainBytes.Length);
            return Convert.ToBase64String(SecureBytes);
        }

        /// <summary>
        /// Encrypts a byte array with AES
        /// </summary>
        /// <param name="plainBytes">Data to encrypt</param>
        /// <returns>Encrypted byte array</returns>
        public byte[] Encrypt(byte[] PlainBytes)
        {
            // Encrypt bytes
            return encryptor.TransformFinalBlock(PlainBytes, 0, PlainBytes.Length);
        }

        /// <summary>
        /// Decrypts a string with AES
        /// </summary>
        /// <param name="SecureText">Encrypted string to decrypt</param>
        /// <returns>Decrypted string</returns>
        public string Decrypt(string SecureText)
        {
            // Convert encrypted string to bytes
            byte[] secureBytes = UnicodeEncoding.Unicode.GetBytes(SecureText);

            // Decrypt bytes
            byte[] plainBytes = decryptor.TransformFinalBlock(secureBytes, 0, secureBytes.Length);

            // Return decrypted bytes as a string
            return UnicodeEncoding.Unicode.GetString(plainBytes);
        }

        /// <summary>
        /// Decrypts data with AES
        /// </summary>
        /// <param name="SecureBytes">Encrypted data to decrypt</param>
        /// <returns>Decrypted data</returns>
        public byte[] Decrypt(byte[] SecureBytes)
        {
            // Decrypt bytes
            return decryptor.TransformFinalBlock(SecureBytes, 0, SecureBytes.Length);
        }
    }
}
