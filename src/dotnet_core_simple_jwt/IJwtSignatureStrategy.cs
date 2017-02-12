using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_core_simple_jwt
{
    public interface IJwtSignatureStrategy
    {
        string SecretKey { get; }

        string Encode(JwtToken jwtToken);

        JwtToken Decode(string jwtToken);
    }
}
