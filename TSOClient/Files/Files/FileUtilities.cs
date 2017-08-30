using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace Files
{
    public static class FileUtilities
    {
        /// <summary>
        /// Generates a hash from a filename.
        /// </summary>
        /// <param name="Filename">The filename to hash.</param>
        /// <returns>A hash as an array of bytes.</returns>
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
        /// A simple pattern matcher for finding byte patterns in streams.
        /// Written by Rhys Simpson.
        /// </summary>
        /// <param name="stream">Stream to search.</param>
        /// <param name="pattern">Pattern to match.</param>
        /// <returns>Position of pattern in stream.</returns>
        public static int BinaryContains(this Stream stream, byte[] pattern)
        {
            stream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < stream.Length; i++)
            {
                byte B = (byte)stream.ReadByte();
                if (B == pattern[0])
                {
                    bool Match = true;
                    for (int j = 1; j < pattern.Length; j++)
                    {
                        var B2 = stream.ReadByte();
                        if (B2 != pattern[j])
                        {
                            Match = false;
                            break;
                        }
                    }
                    if (Match) return (int)stream.Position;
                    else stream.Seek(i + 1, SeekOrigin.Begin);
                }
            }

            return -1;
        }

        /// <summary>
        /// An extension method to get the hash code for arrays of intrinsic types.
        /// </summary>
        /// <typeparam name="T">The type of array.</typeparam>
        /// <param name="Enumerable">The array for which to get the hash code.</param>
        /// <returns>THe hash code for the array.</returns>
        public static int HashCode<T>(this IEnumerable<T> Enumerable)
        {
            int hash = 0x218A9B2C;
            foreach (var item in Enumerable)
            {
                int thisHash = item.GetHashCode();
                //mix up the bits.
                hash = thisHash ^ ((hash << 5) + hash);
            }
            return hash;
        }
    }
}
