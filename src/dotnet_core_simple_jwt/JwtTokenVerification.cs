using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace dotnet_core_simple_jwt
{
    public class JwtTokenVerificaion<T> where T : IdentityUser, new()
    {
        private IJwtSignatureStrategy IJwtSignatureStrategy;
        public JwtTokenVerificaion(string secret, string algorithm = "HS256") 
        {
            this.IJwtSignatureStrategy = JwtSignatureFactory.Create(algorithm, secret);
        }

        public T verify(string jwtToken) 
        {
            try
            {
                var jwt = this.IJwtSignatureStrategy.Decode(jwtToken);
                if(jwt == null || jwt.jwtData.UserId == null) 
                {
                    return null;
                }
                if (jwt.jwtData.Exp < DateTime.UtcNow) 
                {
                    throw new Exception("JWT Token is expired");
                }
                return new T() {
                    UserName = jwt.jwtData.Username,
                    Id = jwt.jwtData.UserId,
                    Email = jwt.jwtData.Username
                };
            }
            catch(Exception e)
            {
                return null;
            }

        }

    }
}