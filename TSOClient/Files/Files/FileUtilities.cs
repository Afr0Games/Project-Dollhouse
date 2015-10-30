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

        /// <summary>
        /// Converts a TypeID in an IFFChunk to a 32bit int.
        /// </summary>
        /// <param name="TypeID">TypeID obtained from an IFFChunk.</param>
        /// <returns>The TypeID converted to a 32bit int.</returns>
        /*public static int BE32(string TypeID)
        {
            return (((TypeID[0]) << 24 | ((TypeID[1]) << 16) | ((TypeID[2]) << 8) | TypeID[3]));
        }*/
    }
}
