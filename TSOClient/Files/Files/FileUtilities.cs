using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Files
{
    public class FileUtilities
    {
        public static byte[] GenerateHash(string Filename)
        {
            byte[] tmpSource;
            byte[] Hash;
            //Create a byte array from source data.
            tmpSource = ASCIIEncoding.ASCII.GetBytes(Filename);
            Hash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            return Hash;
        }
    }
}
