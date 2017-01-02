using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

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

        public async Task<Task> Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            if(path.Value == "/token")
            {
                await this._userManager.FindByEmailAsync("cconnon11@gmail.com");
                if(context.Request.Method == "POST")
                {
                    System.Console.WriteLine("hello");
                }
            }
            return next(context);
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwt<T>(this IApplicationBuilder builder) where T : IdentityUser
        {
            return builder.UseMiddleware<JwtMiddleware<T>>();
        }
    }
}
