using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Linq;

namespace dotnet_core_simple_jwt
{
    public class AuthCreds 
    {
        public string username {get; set;}

        public string password {get; set;}
    }
    public class JwtMiddlewareOptions
    {
        public string secret { get; set; }

    }
    public class JwtMiddleware<T> where T : IdentityUser, new()
    {

        private readonly RequestDelegate next;
        private readonly JwtMiddlewareOptions options;
        private readonly UserManager<T> _userManager;

        public JwtMiddleware(RequestDelegate next,
            IOptions<JwtMiddlewareOptions> options,
            UserManager<T> _userManager)
        {
            this.next = next;
            this.options = options.Value;
            this._userManager = _userManager;
        }

        public async Task Invoke(HttpContext context)
        {
            await HandleRequest(context);
            next(context);
            return;
        }

        public async Task HandleRequest(HttpContext context)
        {
            var path = context.Request.Path;
            if (path.Value == "/token" && context.Request.Method == "POST")
            {
                await HandleTokenRequest(context);
                return;
            }
            if (path.Value == "/register" && context.Request.Method == "POST")
            {
                await HandleRegisterRequest(context);
            }
            return;
        }

        public async Task HandleRegisterRequest(HttpContext context) 
        {
            var json =  JsonConvert.DeserializeObject<AuthCreds>(await ReadAsString(context.Request));
            var user = new T();
            user.UserName = json.username;
            user.Email = json.username;
            var result = await this._userManager.CreateAsync(user, json.password);
            if (result.Succeeded) 
            {
                var jsonString = "{\"success\":\"user created with the provided creds\"}";
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);    
            }
            else 
            {
                System.Console.WriteLine(result.Errors.Select(x => x.Description).ToString());
                var jsonString = "{\"errors\":\"error creating user\"}";
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);    
            }
            return;
        }

        public async Task HandleTokenRequest(HttpContext context)
        {
            var json =  JsonConvert.DeserializeObject<AuthCreds>(await ReadAsString(context.Request));
            var user = await this._userManager.FindByEmailAsync(json.username);
            if (user == null)
            {
                context.Response.StatusCode = 401;
                var jsonString = "{\"error\":\"unauthorized, must pass a correct username and password\"}";
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                return;
            }
            else 
            {
                context.Response.StatusCode = 200;
                var jwtToken = new JwtTokenBuilder().AddSecret(options.secret)
                                                    .AddUsername(user.UserName)
                                                    .AddUserId(user.Id)
                                                    .Build();
                var jsonString = "{\"token\":\""  + jwtToken +"\"}";
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                return;
            }
        }


        public async Task<string> ReadAsString(HttpRequest request)
        {
            var initialBody = request.Body; // Workaround
 
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var body = Encoding.UTF8.GetString(buffer);
            request.Body = initialBody; // Workaround
            return body;
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwt<T>(this IApplicationBuilder builder) where T : IdentityUser, new()
        {
            return builder.UseMiddleware<JwtMiddleware<T>>();
        }
    }
}
