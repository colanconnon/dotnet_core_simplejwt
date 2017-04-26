using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace dotnet_core_simple_jwt
{
    public class JwtTokenVerificaion : IJwtVerificationStrategy
    {
        private IJwtSignatureStrategy IJwtSignatureStrategy;

        private readonly JwtMiddlewareOptions options;
        public JwtTokenVerificaion(
            string secret, 
            JwtMiddlewareOptions jwtOptions, 
            string algorithm = "HS256"
        ) 
        {
            this.options = jwtOptions;
            this.IJwtSignatureStrategy = JwtSignatureFactory.Create(algorithm, secret);
        }


        public IdentityUser HandleTokenVerificationRequest(HttpContext context)
        {
            if(!context.Request.Headers.ContainsKey("Authorization")) 
            {
                throw new InvalidJwtException("No Authorization header provided, this endpoint requires bearer auth");
            }
            var token = context.Request.Headers["Authorization"].ToString().Split(' ');
            if (token[0] != this.options.AuthPrefix) 
            {
                throw new InvalidJwtException("Invalid JWT Token");
            }
            if (token.Length == 2 && token[1] != null) 
            {
                var user = verify(token[1]);
                if ( user != null) 
                {
                    return user;
                }
                else 
                {
                    return null;
                }
            }
            else 
            {
                return null;
            }
        }

        public IdentityUser verify(string jwtToken) 
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
                    throw new InvalidJwtException("JWT Token is expired");
                }
                return new IdentityUser() {
                    UserName = jwt.jwtData.Username,
                    Id = jwt.jwtData.UserId,
                    Email = jwt.jwtData.Username
                };
            }
            catch(InvalidJwtException e) 
            {
                throw new InvalidJwtException(e.Message);   
            }
            catch(Exception e)
            {
                throw new InvalidJwtException("Error verifying jwt token");   
            }

        }

    }
}