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
        public string username { get; set; }

        public string password { get; set; }
    }
    public class JwtMiddlewareOptions
    {
        public string secret { get; set; }

        
        public string AuthPrefix { get; set; } = "Bearer:";

        public string tokenEndpoint { get; set; } = "/token";

        public string registerEndpoint {get; set; } = "/register";

        public bool allowRegistration {get; set;} = true;

        public bool allowTokenIssuing { get; set; } = true;

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
            context.Request.HttpContext.Items.Add("secret", options.secret);
            if (path.Value == options.tokenEndpoint && context.Request.Method == "POST")
            {
                await HandleTokenRequest(context);
                return;
            }
            if (options.allowRegistration && path.Value == options.registerEndpoint && context.Request.Method == "POST")
            {
                await HandleRegisterRequest(context);
            }
            if (path.Value == options.tokenEndpoint && context.Request.Method == "GET")
            {
                var jwtTokenVerification = new JwtTokenVerificaion(options.secret, options);
                var user = jwtTokenVerification.HandleTokenVerificationRequest(context);
                return;
            }
            return;
        }


        public async Task HandleRegisterRequest(HttpContext context)
        {
            var json = JsonConvert.DeserializeObject<AuthCreds>(await ReadAsString(context.Request));
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
                var jsonString = "{\"errors\":\"error creating user\"}";
                context.Response.StatusCode = 400;
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
            }
            return;
        }

        public async Task HandleTokenRequest(HttpContext context)
        {
            var json = JsonConvert.DeserializeObject<AuthCreds>(await ReadAsString(context.Request));
            var user = await this._userManager.FindByEmailAsync(json.username);
            if (user == null)
            {
                context.Response.StatusCode = 401;
                var jsonString = JsonConvert.SerializeObject(new {
                    error = "unauthorized, must pass a correct username and password"
                }).ToString();
                context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                return;
            }
            else
            {
                if (await this._userManager.CheckPasswordAsync(user, json.password))
                {
                    context.Response.StatusCode = 200;
                    var jwtToken = new JwtTokenBuilder().AddSecret(options.secret)
                                                        .AddUsername(user.UserName)
                                                        .AddUserId(user.Id)
                                                        .AddExpiration(DateTime.UtcNow.AddDays(7))
                                                        .Build();
                    var jsonString = "{\"token\":\"" + jwtToken + "\"}";
                    context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                }
                else
                {
                    context.Response.StatusCode = 401;
                    var jsonString = "{\"error\":\"unauthorized, must pass a correct username and password\"}";
                    context.Response.ContentType = new MediaTypeHeaderValue("application/json").ToString();
                    await context.Response.WriteAsync(jsonString, Encoding.UTF8);
                }
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
