using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace dotnet_core_simple_jwt
{
    public class HmacSignatureStrategy : IJwtSignatureStrategy
    {
        public string SecretKey { get; }

        public HmacSignatureStrategy(string secretKey)
        {
            SecretKey = secretKey;
        }

        public JwtToken Decode(string jwtToken)
        {
            string[] jwt = jwtToken.Split('.');
            if (String.IsNullOrEmpty(jwt[0]) || String.IsNullOrEmpty(jwt[1]) || String.IsNullOrEmpty(jwt[2]))
            {
                throw new InvalidJwtException("Malformed jwt token");
            }
            using (var hmac = new HMACSHA256(GetKeyAsBytes()))
            {
                var jwtDataToSign = $"{jwt[0]}.{jwt[1]}";

                var hash = hmac.ComputeHash(
                    jwtDataToSign.ToBytes()
                );
                var jwtSignature = jwt[2].Base64DecodeToBytes();
                if(hash.SequenceEqual(jwtSignature))
                {
                    return new JwtToken()
                    {
                        jwtData = JwtData.Create(jwt[1].Base64Decode()),
                        jwtHeader = JwtHeader.Create(jwt[0].Base64Decode())
                    };
                }
                throw new InvalidJwtException("JWT signature does not match");
            }
        }

        public string Encode(JwtToken jwtToken)
        {
            var base64Payload = jwtToken.jwtData.ToJson().Base64Encode();
            var base64Header = jwtToken.jwtHeader.ToJson().Base64Encode();
            using (var hmac = new HMACSHA256(GetKeyAsBytes()))
            {
                var jwtDataToSign = $"{base64Header}.{base64Payload}";
                var hash = hmac.ComputeHash(
                    jwtDataToSign.ToBytes()
                );
                return $"{jwtDataToSign}.{hash.Base64Encode()}";
            }
        }
        private byte[] GetKeyAsBytes()
        {
            return Encoding.ASCII.GetBytes(this.SecretKey);
        }
    }

}
