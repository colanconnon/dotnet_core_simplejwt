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

namespace dotnet_core_simple_jwt
{
    public class JwtMiddlewareOptions
    {
        public string secret { get; set; }

    }
    public class JwtMiddleware<T> where T : IdentityUser
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
            var path = context.Request.Path;
            if (path.Value == "/token")
            {
                if (context.Request.Method == "POST")
                {
                    System.Console.WriteLine(await ReadAsString(context.Request));
                    var user = await this._userManager.FindByEmailAsync("cconnon11@gmail.com");
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

                    }

                }
            }
            next(context);
            return;
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
 
        // public async Task<T> ReadAsAsync<T>(this HttpRequest request)
        // {
        //     var json = await request.ReadAsString();
        //     T retValue = JsonConvert.DeserializeObject<T>(json);
        //     return retValue;
        // }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwt<T>(this IApplicationBuilder builder) where T : IdentityUser
        {
            return builder.UseMiddleware<JwtMiddleware<T>>();
        }
    }
}
