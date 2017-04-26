using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Microsoft.Extensions.Options;
using System.Security.Principal;

namespace dotnet_core_simple_jwt
{
    public class AuthorizationAttribute : TypeFilterAttribute
    {
        public string Role { get; }

        public AuthorizationAttribute(string roleName) : base(typeof(AuthorizationFilter))
        {
            this.Role = roleName;
        }
    }

    public class AuthorizationFilter : IActionFilter, IFilterContainer
    {
        private readonly JwtMiddlewareOptions options;
        public string role {get; set;}

        public AuthorizationAttribute FilterDefinition
        {
            get
            {
                return (AuthorizationAttribute)((IFilterContainer)this).FilterDefinition;
            }
        }

        IFilterMetadata IFilterContainer.FilterDefinition { get; set; }

        public AuthorizationFilter(IOptions<JwtMiddlewareOptions> options)
        {
            this.options = options.Value;
        }
        

        private Boolean ParseAuthenticationHeader(ActionExecutingContext context) 
        {
                var httpContext = context.HttpContext;
                var JwtTokenVerificaion = new JwtTokenVerificaion(
                    this.options.secret,
                    this.options
                );
                var user = JwtTokenVerificaion.HandleTokenVerificationRequest(httpContext);
                if (user != null) 
                {
                    var principal = new GenericPrincipal(
                        new GenericIdentity(user.UserName, "JWT"),
                        null
                    );
                    httpContext.User = principal;
                    return true;
                }
                return false;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            try 
            {
                if (ParseAuthenticationHeader(context))
                {
                    return;
                } 
                else 
                {
                    var result = new JsonResult(new {Error = "bad request"});   
                    result.StatusCode = 401;
                    context.Result = result;
                }
            }
            catch (InvalidJwtException e)
            {
                    var result = new JsonResult(new {Error = e.Message});   
                    result.StatusCode = 401;
                    context.Result = result;
            }
            catch (Exception e)
            {
                    var result = new JsonResult(new {Error = "bad request"});   
                    result.StatusCode = 400;
                    context.Result = result;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}