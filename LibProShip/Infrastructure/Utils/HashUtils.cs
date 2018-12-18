using System.Security.Cryptography;
using System.Text;

namespace LibProShip.Infrastructure.Utils
{
    public static class HashUtils
    {
        public static string Hash(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                return GetMd5Hash(md5, data);

            }
        }

        static string GetMd5Hash(MD5 md5Hash, byte [] input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(input);

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

      
    }
}