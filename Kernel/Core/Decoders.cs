using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Zazlak
{
    static class Decoders
    {
        public static int DecodeBit24(string v)
        {
            if ((v[0] | v[1] | v[2] | v[3]) < 0)
                return -1;
            return ((v[0] << 24) + (v[1] << 16) + (v[2] << 8) + (v[3] << 0));
        }

        public static int DecodeBit8(string v)
        {
            if ((v[0] | v[1]) < 0)
                return -1;
            return ((v[0] << 8) + (v[1] << 0));
        }

        public static bool ConvertEnumToBool(object Enum)
        {
            if (Enum.ToString() == "1")
                return true;
            else
                return false;
        }

        public static string MD5Encrypt(string value)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();

            byte[] data = System.Text.Encoding.ASCII.GetBytes(value);
            data = provider.ComputeHash(data);

            string md5 = string.Empty;

            for (int i = 0; i < data.Length; i++)
                md5 += data[i].ToString("x2").ToLower();

            return md5;
        }
    }
}
