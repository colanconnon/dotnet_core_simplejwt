using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_core_simple_jwt
{
    public static class JwtSignatureFactory
    {
        public static IJwtSignatureStrategy Create(string strategy, string secret)
        {
            if(strategy == "HS256")
            {
                return new HmacSignatureStrategy(secret);
            }
            else
            {
                throw new Exception("Jwt strategy doesn't exist");
            }
        }
    }
}
