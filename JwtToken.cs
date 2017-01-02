using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_core_simple_jwt
{
    public class JwtToken
    {
        public JwtData jwtData { get; set; }
        public JwtHeader jwtHeader { get; set; }
        public string signature { get; set; }
    }
}
