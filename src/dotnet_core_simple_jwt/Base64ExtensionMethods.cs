using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_core_simple_jwt
{
    public static class Base64ExtensionMethods
    {
        public static string Base64Encode(this String str)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this String str)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(str);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static byte[] ToBytes(this String str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static string Base64Encode(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Base64DecodeToBytes(this string str)
        {
            return Convert.FromBase64String(str);
        }
    }
}
