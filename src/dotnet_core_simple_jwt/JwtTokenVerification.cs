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
    public class JwtTokenVerificaion<T> where T : IdentityUser, new()
    {
        private IJwtSignatureStrategy IJwtSignatureStrategy;
        public JwtTokenVerificaion(string secret, string algorithm = "HS256") 
        {
        
            this.IJwtSignatureStrategy = JwtSignatureFactory.Create(algorithm, secret);
        }

        public async Task HandleTokenVerificationRequest(HttpContext context)
        {
            var token = context.Request.Headers.First(x => x.Key == "Authorization").Value.ToString().Split(' ');
            System.Console.WriteLine(token[0]);
            if (token.Length == 2 && token[1] != null) 
            {
                var user = verify(token[1]);
                if ( user != null) 
                {
                    var jsonString = JsonConvert.SerializeObject(new { Username = user.UserName , Id = user.Id });
                    context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                }
                else 
                {
                    var jsonString  = JsonConvert.SerializeObject(new { Error = "Invalid JWT Token"});
                    context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                }
            }
            else 
            {
                var jsonString = JsonConvert.SerializeObject(new {Error =  "Invalid Authorization header"});
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
            }
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